import React from "react";
import {FeatureItem} from "@site/src/core/types";

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

export default FeatureList;
