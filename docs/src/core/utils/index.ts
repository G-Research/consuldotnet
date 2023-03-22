export default function normalizeVersion(version: string): string {
    /** remove any 0s from the end of the version
     * e.g. 1.0.0 -> 1.0
     */

    return version.replace(/\.0+$/, '');
}
