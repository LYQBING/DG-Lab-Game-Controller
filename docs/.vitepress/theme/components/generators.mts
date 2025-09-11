// 遍历支持的语言，生成语言配置
export const generateLocalesConfig = (languageConfig) =>
{
    const locales = {};
    Object.entries(languageConfig.supported).forEach(([key, config]) =>
    {
		const localeKey = key === languageConfig.default ? 'root' : key; // 默认语言使用根路径
        locales[localeKey] = {
            label: config.label,
            lang: config.lang,
            description: config.description,
            themeConfig: config.themeConfig || {}
        };
    });

    return locales;
};

// 生成重写规则配置，也就是访问的路径
export const generateRewritesConfig = (languageConfig) =>
{
    const defaultLang = languageConfig.default;
    const rewrites = {};

    if (languageConfig.supported[defaultLang])
    {
        const defaultDir = languageConfig.supported[defaultLang].dir;
        rewrites[`${defaultDir}/:rest*`] = ':rest*';
    }

    return rewrites;
};