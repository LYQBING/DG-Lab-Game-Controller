// 生成多语言配置和重写规则配置
export const generateLocalesAndRewritesConfig = (languageConfig) =>
{
	const locales = {};
	const rewrites = {};
	const defaultLang = languageConfig.default;

	Object.entries(languageConfig.supported).forEach(([key, config]) =>
	{
		const localeKey = key === defaultLang ? 'root' : key; // 默认语言使用根路径
		locales[localeKey] = {
			label: config.label,
			lang: config.lang,
			description: config.description,
			themeConfig: config.themeConfig || {}
		};

		// 如果是默认语言，则重写规则为根路径
		if (key === defaultLang)
		{
			rewrites[`${config.dir}/:rest*`] = ':rest*';
		}
	});

	return { locales, rewrites };
};
