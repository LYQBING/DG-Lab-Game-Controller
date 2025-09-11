// 生成侧边栏配置
export const generateSidebarConfigs = (languageConfig) =>
{
	// 获取所有支持的语言
	const supportedLocales = Object.keys(languageConfig.supported);

	// 返回对应的侧边栏配置
	return supportedLocales.map((lang) =>
	{
		const isDefaultLang = lang === languageConfig.default;
		const dirName = languageConfig.supported[lang].dir;

		return {
			manualSortFileNameByPriority: ['Introduction'], // 优先文件
			excludeByGlobPattern: [''], // 黑名单文件
			collapsed: true, // 是否折叠
			useTitleFromFrontmatter: true, // 文件标题
			useFolderTitleFromIndexFile: true, // 文件夹标题

			...(isDefaultLang ? {} : { basePath: `/${lang}/` }),
			documentRootPath: `./docs/${dirName}`,
			resolvePath: isDefaultLang ? '/' : `/${lang}/`,
		};
	});
};