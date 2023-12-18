import React from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import {FeatureItem} from "@site/src/core/types";
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import CodeBlock from '@theme/CodeBlock';
import styles from './Index.module.css';
import LogoSvg from '@site/static/project/logo/svg/Consul.NET_SignatureLogo_RGB-Color.svg';
import badgeList from "@site/src/data/badges";
import FeatureList from "@site/src/data/features";
import MDXContent from '@theme/MDXContent';
import CustomBadge, {DotNetFrameworkBadge, DotNetCoreBadge} from "@site/src/components/CustomBadge";
// @ts-ignore
import SupportedAPIs from '@site/docs/2-guides/3-supported-apis.mdx';

function HomepageBadges() {
    return <div className={clsx(styles.badges)}>{
        [...badgeList.map((props) => <CustomBadge {...props}/>), <DotNetFrameworkBadge/>, <DotNetCoreBadge/>]
            .map((component, idx) => <div key={idx} className={clsx(styles.badge)}>
                {component}
            </div>)}
    </div>
}


function HomepageBanner() {
    const {siteConfig} = useDocusaurusContext();
    return (
        <header className={clsx('hero hero--primary text--center padding-vert--lg', styles.heroBackground)}>
            <div className={clsx('container', styles.heroWrapper)}>
                <div className={clsx(styles.logoSvgBox)}>
                    <LogoSvg fill='transparent' transform="scale(1.3 1.3)" preserveAspectRatio="xMinYMin meet"/>
                </div>
                <h1 className="hero__title" style={{display: 'none'}}>{siteConfig.title}</h1>
                <p className="hero__subtitle">{siteConfig.tagline}</p>
                <HomepageBadges/>
                <CodeBlock language="bash" className="col">dotnet add package Consul</CodeBlock>
                <p>{`Curious about what's next? try the `}
                    <Link className={styles.previewLink} to='/docs/next/'>
                        ⚡ preview version
                    </Link>
                </p>
                <div className={styles.buttons}>
                    <Link
                        className={clsx("button button--info button--lg", styles.darkButton)}
                        to="/docs/">
                        📚 Read the Docs
                    </Link>
                    <Link
                        className={clsx("button button--primary button--lg", styles.darkButton)}
                        to="/docs/category/getting-started/">
                        🚀 Get Started
                    </Link>
                </div>
                <iframe
                    src="https://ghbtns.com/github-btn.html?user=G-Research&repo=consuldotnet&type=star&count=true&size=large"
                    className="margin-top--md padding-left--md"
                    width={170}
                    height={30}
                    title="GitHub Stars"
                />
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
        <section className="padding-top--md padding-bottom--xl text--center">
            <div className="container">
                <div className="row">
                    <div className="col">
                        <header>
                            <h1>Supported APIs</h1>
                        </header>
                        <span className={styles.markdownWrapper}>
                            <MDXContent>
                                <SupportedAPIs/>
                            </MDXContent>
                        </span>
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
