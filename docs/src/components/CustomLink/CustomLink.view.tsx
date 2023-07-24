import React from 'react';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';


export default function CustomLink({href, text}: { href: string, text: string }) {
    return (<a href={href} target="_blank" rel="noopener noreferrer">{text}</a>);
}

export const ConsulDotNetLatestReleaseLink = ({text}: { text: string }) => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const version = customFields?.consulDotNetVersion;
    return <CustomLink
        href={`https://github.com/G-Research/consuldotnet/releases/tag/v${version}`}
        text={text}
    />
};
