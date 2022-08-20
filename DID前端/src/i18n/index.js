import Vue from 'vue';
import Vuei18n from 'vue-i18n';

Vue.use(Vuei18n);
import zh from './lang/zh';
import en from './lang/en';

import { Locale } from 'vant';
import enUS from 'vant/lib/locale/lang/en-US';
import zhCN from 'vant/lib/locale/lang/zh-CN';

const messages = {
	en: {
		...enUS,
		...en
	},
	zh: {
		...zhCN,
		...zh
	}
};
const i18n = new Vuei18n({
	locale: 'zh', // 设置默认语言
	messages: messages // 设置资源文件对象
});

// 更新vant组件库本身的语言变化，支持国际化
function vantLocales(lang) {
	if (lang === 'en') {
		Locale.use(lang, enUS);
	} else if (lang === 'zh') {
		Locale.use(lang, zhCN);
	}
}

export { i18n, vantLocales };
