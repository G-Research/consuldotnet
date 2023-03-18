import React from 'react';
import {BadgeItem} from "@site/src/core/types";
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import defaultMDXStyles from '@docusaurus/theme-classic/lib/theme/MDXComponents/Img/styles.module.css';


function CustomBadge({href, src, alt, inline}: BadgeItem) {
    let component = <img loading="lazy" src={src} alt={alt} className={defaultMDXStyles.img}/>;
    if (href) {
        component = <a href={href} target="_blank" rel="noopener noreferrer">{component}</a>;
    }
    if (!inline) {
        component = <p>{component}</p>;
    }
    return component;
}

CustomBadge.defaultProps = {
    inline: true
}

const ConsulAPIBadge = ({inline}: { inline?: boolean }) => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const version = customFields?.consulAPIVersion;
    return <CustomBadge
        href={`https://github.com/hashicorp/consul/tree/v${version}/api`}
        src={`https://img.shields.io/badge/Consul%20API%20version-${version}-red`}
        alt={`Consul API: ${version}`}
        inline={inline}
    />
};

ConsulAPIBadge.defaultProps = {
    inline: true
}


const DotNetFrameworkBadge = ({inline}: { inline?: boolean }) => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const minVersion = customFields?.dotNetFrameworkMinVersion;
    return <CustomBadge
        src={`https://img.shields.io/badge/.NET%20Framework%20version-%3E=${minVersion}-blue`}
        alt={`.NET Framework: >= ${minVersion}`}
        inline={inline}
    />
};

DotNetFrameworkBadge.defaultProps = {
    inline: true
}

const DotNetCoreBadge = ({inline}: { inline?: boolean }) => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const minVersion = customFields?.dotNetCoreMinVersion;
    return <CustomBadge
        src={`https://img.shields.io/badge/.NET%20Core%20version-%3E=${minVersion}-blueviolet`}
        alt={`.NET Core: >= ${minVersion}`}
        inline={inline}
    />
};

DotNetCoreBadge.defaultProps = {
    inline: true
}


export default CustomBadge;
export {ConsulAPIBadge, DotNetFrameworkBadge, DotNetCoreBadge};
