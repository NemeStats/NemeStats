#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace UI.DependencyResolution {
    /// <summary>
    /// The structure map dependency scope.
    /// </summary>
    public class StructureMapDependencyScope : ServiceLocatorImplBase {

        private const string NestedContainerKey = "Nested.Container.Key";
        public StructureMapDependencyScope(IContainer container) {
            if (container == null) {
                throw new ArgumentNullException("container");
            }
            Container = container;
        }

        public IContainer Container { get; set; }

        public IContainer CurrentNestedContainer {
            get {
                return (IContainer)HttpContext.Items[NestedContainerKey];
            }
            set {
                HttpContext.Items[NestedContainerKey] = value;
            }
        }

        private HttpContextBase HttpContext {
            get {
                var ctx = Container.TryGetInstance<HttpContextBase>();
                return ctx ?? new HttpContextWrapper(System.Web.HttpContext.Current);
            }
        }

        public void CreateNestedContainer() {
            if (CurrentNestedContainer != null) {
                return;
            }
            CurrentNestedContainer = Container.GetNestedContainer();
        }

        public void Dispose() {
            DisposeNestedContainer();
            Container.Dispose();
        }

        public void DisposeNestedContainer() {
            if (CurrentNestedContainer != null) {
                CurrentNestedContainer.Dispose();
				CurrentNestedContainer = null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return DoGetAllInstances(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return (CurrentNestedContainer ?? Container).GetAllInstances(serviceType).Cast<object>();
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            IContainer container = (CurrentNestedContainer ?? Container);

            if (string.IsNullOrEmpty(key)) {
                return serviceType.IsAbstract || serviceType.IsInterface
                    ? container.TryGetInstance(serviceType)
                    : container.GetInstance(serviceType);
            }

            return container.GetInstance(serviceType, key);
        }
    }
}
