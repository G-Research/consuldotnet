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
    // {
    //     alt: 'Feedz',
    //     src: 'https://img.shields.io/feedz/vpre/consuldotnet/preview/consul',
    //     href: '#preview-versions',
    // },
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
    // {
    //     alt: 'Twitter Follow',
    //     // src: "https://img.shields.io/twitter/follow/oss_gr.svg?label=Follow%20@oss_gr&style=social",
    //     src: "https://img.shields.io/twitter/follow/oss_gr.svg?label=Twitter",
    //     href: 'https://twitter.com/oss_gr',
    // }
    // {
    //     alt: '.NET Framework: >= 4.6.1',
    //     src: 'https://img.shields.io/badge/.NET%20version-%3E=4.6.1-blue',
    //     href: 'https://www.nuget.org/packages/Consul',
    // },
    // {
    //     alt: '.NET Core: >= 2.0.0',
    //     src: 'https://img.shields.io/badge/.NET%20Core%20version-%3E=2.0.0-blueviolet',
    //     href: 'https://www.nuget.org/packages/Consul',
    // },
];

export default badgeList;
