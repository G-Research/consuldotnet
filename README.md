[![Consul.NET](https://user-images.githubusercontent.com/18899702/219622225-1fda8125-ed7e-4e07-8281-64d3cd5368b8.png)](https://consuldot.net/)

[![G-Research](https://github.com/G-Research/brand/raw/main/badge/badge.svg)](https://github.com/G-Research)
[![CI](https://github.com/G-Research/consuldotnet/actions/workflows/ci.yml/badge.svg?branch=master&event=push)](https://github.com/G-Research/consuldotnet/actions/workflows/ci.yml?query=branch%3Amaster+event%3Apush)
[![NuGet](https://img.shields.io/nuget/vpre/consul)](https://www.nuget.org/packages/Consul/absoluteLatest)
[![Downloads](https://img.shields.io/nuget/dt/consul?label=Downloads)](https://www.nuget.org/packages/Consul/absoluteLatest)

[![Contribute with GitPod](https://img.shields.io/badge/Contribute%20with-Gitpod-908a85?logo=gitpod)](https://gitpod.io/#https://github.com/G-Research/consuldotnet/)
[![Contributors](https://img.shields.io/github/contributors/G-Research/consuldotnet.svg?label=Contributors)](https://github.com/G-Research/consuldotnet/graphs/contributors)
[![License](https://img.shields.io/github/license/G-Research/consuldotnet.svg?label=License)](https://github.com/G-Research/consuldotnet/blob/master/LICENSE)
[![Twitter Follow](https://img.shields.io/twitter/follow/oss_gr.svg?label=Twitter)](https://twitter.com/oss_gr)

[![Consul API: 1.7.14](https://img.shields.io/badge/Consul%20API%20version-1.7.14-red)](https://github.com/hashicorp/consul/tree/v1.7.14/api)
![.NET: >= 4.6.1](https://img.shields.io/badge/.NET%20version-%3E=4.6.1-blue)
![.NET Core: >= 2.0.0](https://img.shields.io/badge/.NET%20Core%20version-%3E=2.0.0-blueviolet)

Consul.NET is a .NET client library for the [Consul HTTP API](https://www.consul.io/).

> *For further information, please visit the [🌐 Consul.NET website](https://consuldot.net/).*

## 📢 Introduction

Consul.NET is a .NET port of the Go Consul API, but reworked to use .NET idioms such as Tasks/CancellationTokens instead
of Goroutines/Channels. The majority of the calls directly track
the [HTTP API](https://www.consul.io/docs/agent/http.html), but this API does have additional functionality that is
provided in the Go API, like Locks and Semaphores.

**[📖 Learn more about Consul.NET](https://consuldot.net/)**
• **[📚 Documentation](https://consuldot.net/docs/)**

## 📦 Installation

Consul.NET is available as a [NuGet package](https://www.nuget.org/packages/Consul/).

```bash
dotnet add package Consul
```

**[🚀 Getting Started](https://consuldot.net/docs/category/getting-started)**
• **[🆕 Preview version](https://consuldot.net/docs/next/)**

## 💕 Community

If you have any questions, feature requests or bug reports, feel free to open an issue or a pull request.

- [Website](https://consuldot.net/) (Documentation)
- [GitHub Issues](https://github.com/G-Research/consuldotnet/issues) (Bug reports, feature requests, questions)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/c%23+consul) (Questions)
- [Twitter](https://twitter.com/oss_gr) (Latest news)
- [G-Research Open-Source](https://opensource.gresearch.co.uk/) (More open-source projects)

## 🤝 Contributing

We welcome contributions to Consul.NET. Please see
our [Contributing guide](https://consuldot.net/docs/category/contributing) for more information.

**[⚡ Contributing](https://consuldot.net/docs/category/contributing)**
• **[📜 Code of Conduct](https://consuldot.net/docs/contributing/code-of-conduct)**

🙌 Thanks goes to these wonderful people:

[![Contributors](https://contrib.rocks/image?repo=G-Research/consuldotnet)](https://github.com/G-Research/consuldotnet/graphs/contributors)

## 📄 License

Consul.NET is licensed under the [Apache License, Version 2.0](LICENSE).
