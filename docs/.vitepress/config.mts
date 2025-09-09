import { defineConfig } from 'vitepress';
import { withSidebar } from 'vitepress-sidebar';

const vitePressOptions =
{
	title: "DG-Lab Game Controller",
	description: "一个希望将全部游戏郊狼化的控制器：让所有游戏都和郊狼 DG-Lab 一起发电吧！",
	head: [
		['link', { rel: 'icon', type: 'image/svg+xml', href: '/logo.svg' }],
		['link', { rel: 'icon', type: 'image/png', href: '/logo.png' }],
		['meta', { name: 'keywords', content: 'DG-Lab,dglab,郊狼游戏控制器,郊狼连接游戏,郊狼游戏,郊狼,连接,游戏' }],
		['meta', { name: 'author', content: 'LYQBING' }]],

	cleanUrls: true,
	themeConfig:
	{
		logo: '/logo.svg',

		nav: [
			{ text: '首页', link: '/' },
			{ text: '维基', link: 'https://github.com/LYQBING/DG-Lab-Game-Controller/wiki' },
			{ text: '开发', link: '/ModuleApi' }
		],

		socialLinks: [
			{ icon: 'github', link: 'https://github.com/LYQBING/DG-Lab-Game-Controller' }
		],

		footer: {
			message: 'DG-Lab Game Controller',
			copyright: 'Copyright © 2024-2025 LYQBING'
		},

		darkModeSwitchLabel: '暗色模式',
		outlineTitle: '文档目录',
		returnToTopLabel: '返回顶部',
		lastUpdatedText: '上次更新',
		docFooter:
		{
			prev: '上一篇',
			next: '下一篇'
		},

		notFound:
		{
			code: '404',
			title: '这究竟是哪里呢',
			quote: '这里已经被遗忘了哦，即使主人您再怎么寻找线索，也是没有任何意义的哦',
			linkLabel: '返回链接',
			linkText: '返回首页'
		},
	}
};

const vitePressSidebarOptions =
{
	documentRootPath: '/docs/',// 文档根目录
	manualSortFileNameByPriority: [''], // 优先排序的文件
	excludeByGlobPattern: [''], // 排除的文件
	collapsed: true, // 是否默认折叠
	useTitleFromFrontmatter: true, // 根据 title 显示侧边栏标题
	useFolderTitleFromIndexFile: true, // 使用文件夹中的 index.md 作为标题
};

export default defineConfig(withSidebar(vitePressOptions, vitePressSidebarOptions));
