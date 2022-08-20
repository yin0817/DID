import Vue from 'vue';
import Vuex from 'vuex';

import { getItem, setItem } from '@/utils/storage';

import { baseUrl } from '@/utils/request';

Vue.use(Vuex);

const state = {
	url: baseUrl,
	merchantInfoMap: {},
	userUid: getItem('uid'),
	zh: {}
};
const actions = {};

const mutations = {
	setMerchantInfoMap(state, data) {
		state.merchantInfoMap[data.it.odid] = data;
	},
	setZH(state, data) {
		console.log(state, data);
	},
	setRoute() {
		this.$router.back();
	}
};

const getters = {};

export default new Vuex.Store({
	state,
	actions,
	mutations,
	getters,
	strict: true
});
