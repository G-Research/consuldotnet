import {BadgeItem} from "@site/src/core/types";

const badgeList: BadgeItem[] = [
    {
        alt: 'Downloads',
        src: 'https://img.shields.io/nuget/dt/consul?label=Downloads',
        href: 'https://www.nuget.org/packages/Consul/absoluteLatest',
    },
    {
        alt: 'NuGet',
        src: 'https://img.shields.io/nuget/vpre/consul',
        href: 'https://www.nuget.org/packages/Consul/absoluteLatest',
    },
    {
        alt: 'CI',
        src: 'https://github.com/G-Research/consuldotnet/actions/workflows/ci.yml/badge.svg?branch=master&event=push',
        href: 'https://github.com/G-Research/consuldotnet/actions/workflows/ci.yml?query=branch%3Amaster+event%3Apush',
    },
    {
        alt: 'License',
        src: 'https://img.shields.io/github/license/G-Research/consuldotnet.svg?label=License',
        href: 'https://github.com/G-Research/consuldotnet/blob/master/LICENSE',
    },
    {
        alt: 'Contributors',
        src: 'https://img.shields.io/github/contributors/G-Research/consuldotnet.svg?label=Contributors',
        href: 'https://github.com/G-Research/consuldotnet/graphs/contributors',
    },
    {
        alt: 'Contribute with GitPod',
        src: 'https://img.shields.io/badge/Contribute%20with-Gitpod-908a85?logo=gitpod',
        href: 'https://gitpod.io/#https://github.com/G-Research/consuldotnet/',
    },
];

export default badgeList;
