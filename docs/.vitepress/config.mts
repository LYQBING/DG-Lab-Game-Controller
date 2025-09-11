import { defineConfig } from 'vitepress';
import { withSidebar } from 'vitepress-sidebar';

import languageConfig from './locales/index.js';
import { generateLocalesAndRewritesConfig } from './theme/components/generators.mts';
import { generateSidebarConfigs } from './theme/components/sidebar.mts';

const SiteUrl = 'https://dg.lyqbing.top/';
const { locales: localesConfig, rewrites: rewritesConfig } = generateLocalesAndRewritesConfig(languageConfig);
const sidebarConfigs = generateSidebarConfigs(languageConfig);

const vitePressOptions = 
{
	title: "DG-Lab Game Controller",
	locales: localesConfig,
	rewrites: rewritesConfig,
	lastUpdated: true,
	cleanUrls: true,
	metaChunk: true,

	head: [
		['link', { rel: 'icon', type: 'image/svg+xml', href: '/logo.svg' }],
		['link', { rel: 'icon', type: 'image/png', href: '/logo.png' }],

		['meta', { property: 'og:type', content: 'website' }],
		['meta', { property: 'og:site_name', content: 'DG-Lab Game Controller' }],
		['meta', { property: 'og:image', content: `${SiteUrl}DG-Lab-Game-Controller.png` }],
		['meta', { property: 'og:url', content: SiteUrl }],

		['meta', { name: 'keywords', content: 'DG-Lab,dglab,dg lab,DG-Lab Game Controller,郊狼游戏控制器,郊狼连接游戏,郊狼游戏,郊狼玩具,郊狼,连接,游戏' }],
		['meta', { name: 'author', content: 'LYQBING' }],
	],

	sitemap: { hostname: SiteUrl },

	themeConfig: {
		logo: { src: '/logo.svg', width: 24, height: 24 },
		socialLinks: [{ icon: 'github', link: 'https://github.com/LYQBING/DG-Lab-Game-Controller' }],

		search: {
			provider: 'local',
		},

		footer: {
			message: 'DG-Lab Game Controller',
			copyright: 'Copyright © 2024-2025 LYQBING'
		},
	}
};

export default defineConfig(withSidebar(vitePressOptions, sidebarConfigs));
