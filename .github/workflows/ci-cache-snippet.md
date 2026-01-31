# GitHub Actions Caching for NemeStats
# This workflow step is for NuGet package caching to speed up builds.

# Add this step after 'Setup NuGet' and before 'Restore NuGet packages' in your workflow:
#
#    - name: Cache NuGet packages
#      uses: actions/cache@v4
#      with:
#        path: ${{ env.NUGET_PACKAGES }}
#        key: nuget-${{ runner.os }}-${{ hashFiles('**/packages.config') }}
#        restore-keys: |
#          nuget-${{ runner.os }}-
