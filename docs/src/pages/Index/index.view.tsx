import React from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import {FeatureItem, BadgeItem} from "@site/src/types";
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import CodeBlock from '@theme/CodeBlock';
import styles from './index.module.css';
import ConsulDotNetLogoSvg from '@site/static/project/logo/svg/Consul.NET_SignatureLogo_RGB-Color.svg';
import badgeList from "@site/src/data/badges";
import FeatureList from "@site/src/data/features";
import MDXContent from '@theme/MDXContent';
// @ts-ignore
import SupportedAPIs from '@site/docs/2-guides/3-supported-apis.mdx';

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
                    <Link className={styles.previewLink} to='/docs/next/'>
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
                        src="https://ghbtns.com/github-btn.html?user=G-Research&repo=consuldotnet&type=star&count=true&size=large"
                        width={170}
                        height={30}
                        title="GitHub Stars"
                    />
                </div>
            </div>
        </header>
    );
}


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

function HomepageCompatibilities(): JSX.Element {
    return (
        <section className={styles.compatibilities}>
            <div className="container">
                <div className="text--center">
                    <h1>Supported APIs</h1>
                </div>
                <div className="row">
                    <div className="col">
                        <MDXContent>
                            <SupportedAPIs/>
                        </MDXContent>
                    </div>
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
                <HomepageCompatibilities/>
            </main>
        </Layout>
    );
}
