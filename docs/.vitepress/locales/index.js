import zh from './zh.js';
import en from './en.js';
import jp from './jp.js';

export default {
	default: 'zh',
	supported: {
		zh: {
			label: '简体中文',
			lang: 'zh-Hans',
			dir: 'zh',
			...zh
		},
		en: {
			label: 'English',
			lang: 'en',
			dir: 'en',
			...en
		},
		jp: {
			label: '日本語',
			lang: 'jp',
			dir: 'jp',
			...jp
		}
	}
};