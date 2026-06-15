# Azure Deployment Setup

This repository deploys through GitHub Actions without committing a `.pubxml` file or any Azure identifiers.

## GitHub Configuration

Create this GitHub environment:

- `production`

Add a required reviewer to the `production` environment so production deployments require approval.

Enable branch protection for `master` and require the `CI` workflow to pass before merge.

## Required Secrets

Add these secrets to the `production` environment:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_WEBAPP_NAME`
- `PRODUCTION_SMOKE_TEST_URL`

`PRODUCTION_SMOKE_TEST_URL` should point at a lightweight production URL that returns a successful response when the deployment is healthy.

## Azure Authentication

Use GitHub OIDC with an Azure service principal:

1. Create or choose an Azure AD app registration / service principal.
2. Grant it permission to deploy to the target App Service.
3. Add a federated credential that trusts this GitHub repository and the deploy workflow.
4. Copy the client ID, tenant ID, subscription ID, app name, and smoke test URL into the GitHub environment secrets listed above.

## Workflow Behavior

On a push to `master`, the deploy workflow will:

1. Build the solution.
2. Run non-integration tests.
3. Publish `Source/UI/UI.csproj` and package it as a deployment artifact.
4. Wait for production environment approval.
5. Deploy that already-built package to the production App Service.
6. Run the production smoke test.

## Notes

- No Azure subscription IDs, app names, smoke test URLs, or publish profiles are stored in git.
- The web app publish step uses MSBuild filesystem publish and does not rely on any checked-in `.pubxml`.
- The `UI` project already includes `Properties/webjobs-list.json`, so the continuous WebJob should publish with the site artifact.
- This workflow is designed for App Service tiers such as `Shared D1` that do not support deployment slots.
