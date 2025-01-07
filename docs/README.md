[![Consul.NET](https://user-images.githubusercontent.com/18899702/219622225-1fda8125-ed7e-4e07-8281-64d3cd5368b8.png)](https://consuldot.net/)

This is the official website for the Consul.NET project, published at [🌐 Website](https://consuldot.net/).

> This website is built using the static website generator [Docusaurus 2](https://docusaurus.io/). Read
> the [Docusaurus documentation](https://docusaurus.io/docs) to learn more.

## Installation

```shell
yarn install
```

## Local Development

```shell
yarn start
```

This command starts a local development server and opens up a browser window. Most changes are reflected live without
having to restart the server.

### Generating the "API Reference" section

You can generate an API Reference section automatically based on the source code
using [DocFX](https://dotnet.github.io/docfx/) by running the following commands:

```shell
# restore the DocFX tool
dotnet tool restore

# generate markdown files using DocFX
yarn run generate-api-folder

# transform the generated markdown files
yarn run transform-api-folder

# or using one command that does both
yarn run api:generate

# to clean the generated files
yarn run api:clean
```

## Build

```shell
yarn build
```

This command generates static content into the `build` directory and can be served using any static contents hosting
service.

## Deployment

This website is built and deployed automatically using GitHub Actions on every push to the `master` branch.
