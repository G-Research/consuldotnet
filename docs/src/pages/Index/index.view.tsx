import React from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import {FeatureItem, BadgeItem} from "@site/src/types";
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import CodeBlock from '@theme/CodeBlock';
import styles from './index.module.css';
import ConsulDotNetLogoSvg from '@site/static/project/logo/svg/Consul.NET_SignatureLogo_RGB-Color.svg';

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
];

function Badge({alt, src, href}: BadgeItem) {
    return (
        <div className={clsx(styles.badge)}>
            <a href={href} target="_blank" rel="noopener noreferrer">
                <img src={src} alt={alt}/>
            </a>
        </div>
    );
}

function HomepageBadges() {
    return <div className={clsx(styles.badges)}>{
        badgeList.map((props, idx) => <Badge key={idx} {...props}/>)}
    </div>
}


function HomepageBanner() {
    const {siteConfig} = useDocusaurusContext();
    return (
        <header className={clsx('hero hero--primary', styles.heroBanner)}>
            <div className="container">
                {/*<img src={require('@site/static/project/logo/png/Consul.NET_SignatureLogo_RGB-Color.png').default}*/}
                {/*     alt="Consul.NET logo"*/}
                {/*     height="120"*/}
                {/*     width="560"*/}
                {/*     style={{objectFit: 'cover', objectPosition: 'center'}}*/}
                {/*/>*/}
                <div className={clsx(styles.logoContainer)}>
                    <ConsulDotNetLogoSvg fill='transparent' transform="scale(1.8 1.8)"/>
                </div>
                <h1 className="hero__title" style={{display: 'none'}}>{siteConfig.title}</h1>
                <p className="hero__subtitle">{siteConfig.tagline}</p>
                <HomepageBadges/>
                <CodeBlock language="bash">
                    {`dotnet add package Consul`}
                </CodeBlock>
                <p>{`Curious about what's next? try the `}
                    <Link className={styles.previewLink} to='/docs/'>
                        preview version
                    </Link>
                </p>
                <div className={styles.buttons}>
                    <Link
                        className={clsx("button button--info button--lg", styles.darkButton)}
                        to="/docs/">
                        Get Started
                    </Link>
                    <Link
                        className={clsx("button button--primary button--lg", styles.darkButton)}
                        to="/docs/">
                        Try it on GitPod
                    </Link>
                </div>
                <div className={clsx('margin-top--md')}>
                    <iframe
                        src="https://ghbtns.com/github-btn.html?user=G-Research&amp;repo=consuldotnet&amp;type=star&amp;count=true&amp;size=large"
                        width={132}
                        height={30}
                        title="GitHub Stars"
                    />
                </div>
            </div>
        </header>
    );
}


const FeatureList: FeatureItem[] = [
    {
        title: 'Easy to Use',
        Svg: require('@site/static/images/easy.svg').default,
        description: (
            <>
                Consul.NET was designed by humans, for humans. It's meant to be intuitive, simple and easy to use.
            </>
        ),
    },
    {
        title: 'Open-Source',
        Svg: require('@site/static/images/community.svg').default,
        description: (
            <>
                Consul.NET is open-source and free to use. It's developed by the community and for the community.
            </>
        ),
    },
    {
        title: 'Production Ready',
        Svg: require('@site/static/images/ready.svg').default,
        description: (
            <>
                Consul.NET is used in production by many companies and is battle-tested.
            </>
        ),
    },
];

function Feature({title, Svg, description}: FeatureItem) {
    return (
        <div className={clsx('col col--4')}>
            <div className="text--center">
                <Svg className={styles.featureIcon} role="img"/>
            </div>
            <div className="text--center padding-horiz--md">
                <h3>{title}</h3>
                <p>{description}</p>
            </div>
        </div>
    );
}

function HomepageFeatures(): JSX.Element {
    return (
        <section className={styles.features}>
            <div className="container">
                <div className="row">
                    {FeatureList.map((props, idx) => (
                        <Feature key={idx} {...props} />
                    ))}
                </div>
            </div>
        </section>
    );
}

export default function Home(): JSX.Element {
    const {siteConfig} = useDocusaurusContext();
    return (
        <Layout
            title={`${siteConfig.title}`}
            description={`${siteConfig.tagline}`}>
            <HomepageBanner/>
            <main>
                <HomepageFeatures/>
            </main>
        </Layout>
    );
}
