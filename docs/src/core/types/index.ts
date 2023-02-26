import React from "react";

export type FeatureItem = {
    title: string;
    Svg: React.ComponentType<React.ComponentProps<'svg'>>;
    description: JSX.Element;
};

export type BadgeItem = {
    src: string;
    alt: string;
    href?: string;
    inline?: boolean;
};
