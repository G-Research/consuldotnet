FROM gitpod/workspace-full:latest

USER gitpod

# Install .NET SDK
# Same version as: https://github.com/g-research/consuldotnet/blob/master/global.json
# Docs: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script#options
# Source: https://docs.microsoft.com/dotnet/core/install/linux-scripted-manual#scripted-install
RUN mkdir -p /home/gitpod/dotnet && curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 6.0 --version latest --install-dir /home/gitpod/dotnet
ENV DOTNET_ROOT=/home/gitpod/dotnet
ENV PATH=$PATH:/home/gitpod/dotnet
