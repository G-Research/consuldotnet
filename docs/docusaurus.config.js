// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require('prism-react-renderer/themes/github');
const darkCodeTheme = require('prism-react-renderer/themes/dracula');

require('dotenv').config();

const dotNetFrameworkMinVersion = `4.6.1`;
const dotNetCoreMinVersion = `2.0.0`;
const consulDotNetVersion = clean_version(process.env.CONSUL_DOT_NET_VERSION || `X.X.X.X`);
const consulAPIVersion = clean_version(extract_consul_version(consulDotNetVersion));

function clean_version(version) {
    if (version) {
        return version.replace(/^v/, ``);
    }
    return ``;
}

function extract_consul_version(consul_dot_net_version) {
    if (consul_dot_net_version) {
        const parts = consul_dot_net_version.split(".");
        return parts.slice(0, 3).join(".");
    }
    return ``;
}

const announcementBarContent = `⭐️ If you like Consul.NET, give it a star on
<a target="_blank" rel="noopener noreferrer" href="https://github.com/G-Research/consuldotnet">GitHub</a>
<svg width="20" height="20" fill="#171515" style="vertical-align: middle; margin-left: 2px;" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32"><path d="M16 1.375c-8.282 0-14.996 6.714-14.996 14.996 0 6.585 4.245 12.18 10.148 14.195l0.106 0.031c0.75 0.141 1.025-0.322 1.025-0.721 0-0.356-0.012-1.3-0.019-2.549-4.171 0.905-5.051-2.012-5.051-2.012-0.288-0.925-0.878-1.685-1.653-2.184l-0.016-0.009c-1.358-0.93 0.105-0.911 0.105-0.911 0.987 0.139 1.814 0.718 2.289 1.53l0.008 0.015c0.554 0.995 1.6 1.657 2.801 1.657 0.576 0 1.116-0.152 1.582-0.419l-0.016 0.008c0.072-0.791 0.421-1.489 0.949-2.005l0.001-0.001c-3.33-0.375-6.831-1.665-6.831-7.41-0-0.027-0.001-0.058-0.001-0.089 0-1.521 0.587-2.905 1.547-3.938l-0.003 0.004c-0.203-0.542-0.321-1.168-0.321-1.821 0-0.777 0.166-1.516 0.465-2.182l-0.014 0.034s1.256-0.402 4.124 1.537c1.124-0.321 2.415-0.506 3.749-0.506s2.625 0.185 3.849 0.53l-0.1-0.024c2.849-1.939 4.105-1.537 4.105-1.537 0.285 0.642 0.451 1.39 0.451 2.177 0 0.642-0.11 1.258-0.313 1.83l0.012-0.038c0.953 1.032 1.538 2.416 1.538 3.937 0 0.031-0 0.061-0.001 0.091l0-0.005c0 5.761-3.505 7.029-6.842 7.398 0.632 0.647 1.022 1.532 1.022 2.509 0 0.093-0.004 0.186-0.011 0.278l0.001-0.012c0 2.007-0.019 3.619-0.019 4.106 0 0.394 0.262 0.862 1.031 0.712 6.028-2.029 10.292-7.629 10.292-14.226 0-8.272-6.706-14.977-14.977-14.977-0.006 0-0.013 0-0.019 0h0.001z"></path></svg>
and follow us on <a target="_blank" rel="noopener noreferrer" href="https://twitter.com/oss_gr">Twitter</a>
<svg width="20" height="20" fill="#00aaec" style="vertical-align: middle; margin-left: 2px;" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M459.37 151.716c.325 4.548.325 9.097.325 13.645 0 138.72-105.583 298.558-298.558 298.558-59.452 0-114.68-17.219-161.137-47.106 8.447.974 16.568 1.299 25.34 1.299 49.055 0 94.213-16.568 130.274-44.832-46.132-.975-84.792-31.188-98.112-72.772 6.498.974 12.995 1.624 19.818 1.624 9.421 0 18.843-1.3 27.614-3.573-48.081-9.747-84.143-51.98-84.143-102.985v-1.299c13.969 7.797 30.214 12.67 47.431 13.319-28.264-18.843-46.781-51.005-46.781-87.391 0-19.492 5.197-37.36 14.294-52.954 51.655 63.675 129.3 105.258 216.365 109.807-1.624-7.797-2.599-15.918-2.599-24.04 0-57.828 46.782-104.934 104.934-104.934 30.213 0 57.502 12.67 76.67 33.137 23.715-4.548 46.456-13.32 66.599-25.34-7.798 24.366-24.366 44.833-46.132 57.827 21.117-2.273 41.584-8.122 60.426-16.243-14.292 20.791-32.161 39.308-52.628 54.253z"></path></svg>
`;

/** @type {import('@docusaurus/types').Config} */
const config = {
    title: 'Consul.NET',
    tagline: '.NET client library for the Consul HTTP API',
    favicon: 'favicon.ico',
    onBrokenLinks: 'throw',
    onBrokenMarkdownLinks: 'warn',

    url: 'https://consuldot.net',
    baseUrl: '/',

    // GitHub pages deployment config.
    // If you aren't using GitHub pages, you don't need these.
    organizationName: 'G-Research', // Usually your GitHub org/user's name.
    projectName: 'consuldotnet', // Usually your repo name.

    customFields: {
        consulDotNetVersion,
        consulAPIVersion,
        dotNetFrameworkMinVersion,
        dotNetCoreMinVersion,
    },

    i18n: {
        defaultLocale: 'en',
        locales: ['en'],
    },

    presets: [
        [
            'classic',
            /** @type {import('@docusaurus/preset-classic').Options} */
            ({
                docs: {
                    sidebarPath: require.resolve('./sidebars.js'),
                    editUrl: 'https://github.com/G-Research/consuldotnet/edit/master/docs/',
                },
                theme: {
                    customCss: require.resolve('./src/css/custom.css'),
                },
                gtag: {
                    trackingID: 'G-P4NS0NP9ZM',
                    anonymizeIP: true,
                },
            }),
        ],
    ],

    themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
        ({
            navbar: {
                title: 'Consul.NET',
                logo: {
                    alt: 'Consul.NET Logo',
                    src: 'project/logo/svg/Consul.NET_IconLogo_RGB-Black.svg',
                    srcDark: 'project/logo/svg/Consul.NET_IconLogo_RGB-Color.svg',
                },
                items: [
                    // left
                    {
                        docId: 'README',
                        type: 'doc',
                        label: 'Docs',
                        position: 'left',
                    },
                    {
                        to: '/docs/category/contributing',
                        label: 'Contribute',
                        position: 'left',
                        activeBaseRegex: `dummy-never-match`,
                    },
                    {
                        to: '/docs/support',
                        label: 'Community',
                        position: 'left',
                        activeBaseRegex: `dummy-never-match`,
                    },
                    // right
                    {
                        href: 'https://github.com/G-Research/consuldotnet',
                        position: 'right',
                        className: 'header-github-link',
                        'aria-label': 'GitHub repository',
                    },
                    {
                        href: 'https://twitter.com/oss_gr',
                        position: 'right',
                        className: 'header-twitter-link',
                        'aria-label': 'Twitter',
                    },
                ],
            },
            footer: {
                links: [
                    {
                        title: 'Learn',
                        items: [
                            {
                                label: 'Introduction',
                                to: '/docs/',
                            },
                            {
                                label: 'Getting Started',
                                to: '/docs/category/getting-started',
                            },
                            {
                                label: 'Guides',
                                to: '/docs/category/guides',
                            },
                        ],
                    },
                    {
                        title: 'Contribute',
                        items: [
                            {
                                label: 'Overview',
                                to: '/docs/category/contributing',
                            },
                            {
                                label: 'Found an Issue?',
                                to: '/docs/contributing/report-issue',
                            },
                            {
                                label: 'Want a Feature?',
                                to: '/docs/contributing/request-feature',
                            },
                        ],
                    },
                    {
                        title: 'Community',
                        items: [
                            {
                                label: 'Help',
                                to: '/docs/support',
                            },
                            {
                                label: 'Stack Overflow',
                                href: 'https://stackoverflow.com/questions/tagged/c%23+consul',
                            },
                            {
                                label: 'GitHub Issues',
                                href: 'https://github.com/G-Research/consuldotnet/issues',
                            },
                        ],
                    },
                    {
                        title: 'More',
                        items: [
                            {
                                label: 'GitHub',
                                href: 'https://github.com/G-Research/consuldotnet',
                            },
                            {
                                label: 'Twitter',
                                href: 'https://twitter.com/oss_gr',
                            },
                            {
                                label: 'G-Research Open-Source',
                                href: 'https://opensource.gresearch.co.uk/',
                            },
                        ],
                    },
                ],
                logo: {
                    alt: 'G-Research Open-Source Software',
                    src: 'organization/logo/svg/GR-OSS.svg',
                    srcDark: 'organization/logo/svg/GR-OSS-Dark.svg',
                    href: 'https://opensource.gresearch.co.uk/',
                },
                copyright: `Copyright © ${new Date().getFullYear()} Consul.NET`,
            },
            announcementBar: {
                // https://docusaurus.io/docs/api/themes/configuration#announcement-bar
                id: 'announcement-bar--support-us-1', // increment on change
                content: announcementBarContent,
                isCloseable: true,
            },
            colorMode: {
                defaultMode: 'light',
                disableSwitch: false,
                respectPrefersColorScheme: true,
            },
            prism: {
                theme: lightCodeTheme,
                darkTheme: darkCodeTheme,
                defaultLanguage: 'csharp',
                additionalLanguages: ['csharp', 'powershell', 'bash'],
            },
            metadata: [{name: 'twitter:card', content: 'summary'}],
        }),
};

module.exports = config;
