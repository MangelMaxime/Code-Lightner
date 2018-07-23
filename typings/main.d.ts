export interface Config {
    backgroundColor?: string;
    textColor?: string;
    grammarFiles: Array<string>;
    scopeName: string;
    themeFile: string;
}

export const lighten : (config : Config, sourceCode : string) => string;