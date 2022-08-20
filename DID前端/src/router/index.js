import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

const router = new VueRouter({
	routes: [
		{ path: '/login', component: () => import('@/view/Login') },
		{
			path: '/',
			component: () => import('@/Home.vue'), //主页
			children: [
				{ path: 'index', component: () => import('@/view/Index/index.vue'), alias: '/' }, //首页
				{ path: 'authentication', component: () => import('@/view/Authentication/index.vue'), name: 'authentication' }, //个人中心
				{ path: 'receiving', component: () => import('@/view/Receiving/index.vue') },
				{ path: 'chain', name: 'chain', component: () => import('@/components/Chain/index.vue') }, // 公链地址信息
				{ path: 'accountstate/erroridentity', name: 'erroridentity', component: () => import('@/components/Accountstate/erroridentity.vue') }
			]
		},
		{ path: '/credit', component: () => import('@/view/Credit/index.vue') } //信用明细
	]
});
router.beforeEach((to, from, next) => {
	next();
});
export default router;
