import React from 'react';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import normalizeVersion from "@site/src/core/utils";


export default function CustomText({text}: { text: string }) {
    return <>{text}</>;
}

export const DotNetSupportedVersion = () => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const minVersion: string = customFields?.dotNetFrameworkMinVersion;
    return <CustomText text={`.NET ${normalizeVersion(minVersion)}+`}/>
};

export const DotNetCoreSupportedVersion = () => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const minVersion: string = customFields?.dotNetCoreMinVersion;
    return <CustomText text={`.NET Core ${normalizeVersion(minVersion)}+`}/>
};

export const DotNetFrameworkDevVersion = () => {
    const {
        siteConfig: {customFields}
    } = useDocusaurusContext();
    // @ts-ignore
    const minVersion: string = customFields?.dotNetFrameworkMinVersion;
    return <CustomText text={`.NET Framework ${normalizeVersion(minVersion)}`}/>
};
