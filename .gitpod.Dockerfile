FROM gitpod/workspace-full:latest

USER gitpod

# Install .NET SDK
ADD global.json ./
RUN mkdir -p /home/gitpod/dotnet \
    && curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --jsonfile global.json --install-dir /home/gitpod/dotnet \
    && rm global.json
ENV DOTNET_ROOT=/home/gitpod/dotnet
ENV PATH=$PATH:/home/gitpod/dotnet

# Install Consul Server
ADD Directory.Build.props ./
RUN CONSUL_VERSION=$(cat Directory.Build.props|grep -oP '(?<VersionPrefix>)\d{1,3}\.\d{1,3}\.\d{1,3}') \
    && CONSUL_ARCH=$(dpkg --print-architecture) \
    && rm Directory.Build.props \
    && echo "Installing Consul ${CONSUL_VERSION}_${CONSUL_ARCH} ..." \
    && cd /usr/local/bin \
    && sudo wget "https://releases.hashicorp.com/consul/${CONSUL_VERSION}/consul_${CONSUL_VERSION}_linux_${CONSUL_ARCH}.zip" \
    && sudo unzip -o "consul_${CONSUL_VERSION}_linux_${CONSUL_ARCH}.zip" \
    && sudo rm "consul_${CONSUL_VERSION}_linux_${CONSUL_ARCH}.zip" \
    && cd \
    && consul --version

# Install Mono JIT compiler
RUN sudo apt install -y gnupg ca-certificates \
    && sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list  \
    && sudo apt update \
    && sudo apt install -y mono-complete \
    && mono --version
