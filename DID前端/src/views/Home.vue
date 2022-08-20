<template>
	<div id="nav">
		<div>
			<van-nav-bar ref="bar" fixed placeholder z-index="10" @click-left="onClickLeft" @click-right="onClickRight" :border="false">
				<template #left>
					<i class="iconfont icon-liebiao" v-if="!show"></i>
					<i class="iconfont icon-quxiao" v-else></i>
				</template>
				<template #title>
					<img class="logo" @click="gohome()" src="@/assets/currency-icons/eotc.png" alt="" />
				</template>
				<template #right v-if="uid != null">
					<i class="iconfont icon-wode"></i>
				</template>
			</van-nav-bar>
		</div>
		<van-popup v-model="show" get-container="#nav" position="left" class="pop">
			<div class="content">
				<div class="item" v-for="(item, index) in list" :key="index" @click="skip(item)">
					<p>
						{{ item.title }}
					</p>
					<img src="../public/img/10.png" alt="" />
				</div>
			</div>
		</van-popup>
		<transition-page>
			<router-view></router-view>
		</transition-page>
	</div>
</template>

<script>
import transitionPage from './components/transitionPage';

export default {
	data() {
		return {
			uid: '',
			flag: false,
			show: false,
			show1: true,

			flag2: true,
			list: [
				{ title: 'EOTC官网', event: 'https://eotc.im' },
				{ title: '链上理财赚币', event: 'https://fi.eotc.im/' },
				{ title: '去中心化币币交易所' },
				{ title: '去中心化借贷交易所' },
				{ title: '去中心化合约交易所' },
				{ title: 'DID去中心化身份系统' },
				{ title: '去中心化应用系统' },
				{ title: 'EOTC NFT', event: 'https://nft.eotc.im/' },
				{ title: 'EOTC 元宇宙' },
				{ title: 'EOTC DAO' }
			]
		};
	},
	methods: {
		gohome() {
			this.show = false;
			this.$router.push({ path: '/index' });
		},
		onClickLeft() {
			this.show = !this.show;
		},
		onClickRight() {
			this.show = false;
			// this.pass();
			if (this.$route.name == 'authentication') {
				this.$router.back();
			} else {
				this.$router.push({
					name: 'authentication'
				});
			}
		},
		skip(item) {
			if (item.event != undefined) {
				const nextType = localStorage.getItem('netType');
				if (nextType === 'bsc' && item.event === 'https://fi.eotc.im/') {
					this.$toast.warning('功能即将上线！');
					return false;
				}
				window.location.href = item.event;
			} else {
				this.$toast.warning('功能即将上线！');
			}
		}
	},
	components: {
		transitionPage
	},
	created() {
		console.log(document.documentElement.clientWidth);
	}
};
</script>

<style lang="less" scoped>
/deep/ .van-nav-bar__title {
	display: flex !important;
	align-content: center;
}

/deep/ .van-nav-bar {
	width: 100%;
	background-color: #1b2945;
	box-sizing: border-box;
	height: auto;
}
/deep/ .van-overlay {
	z-index: 9 !important;
}
/deep/ .van-popup--left {
	top: e('calc(50% + 1.4em)');
}

.pop {
	width: 80%;
	height: 100vh;
	z-index: 10 !important;
	background: #1b2946;

	.content {
		div {
			height: 1.33333rem;
			line-height: 1.33333rem;
			margin: 0 0.64rem;
			border-bottom: 0.01333rem solid #2b374f;
			font-size: 0.37333rem;
			color: #b4b7c2;
			display: flex;
			justify-content: space-between;
			align-items: center;
			img {
				width: 0.42667rem;
				height: 0.42667rem;
			}
		}
	}
}
.container::before {
	content: '';
	position: absolute;
	top: 2.9rem;
	right: 0;
	width: 100%;
	height: 100%;
	background: rgba(76, 76, 76, 0.3);
	backdrop-filter: blur(5px);
	z-index: 9;
}

.container1 {
	width: 100;
	height: 6vh;
	background-color: #1b2942;
	padding: 0 6vw;
	display: flex;
	align-items: center;
	justify-content: space-between;
	box-sizing: border-box;
	.img1 {
		width: 4vw;
		height: 2vh;
	}
	.img2 {
		width: 7vw;
		height: 4vh;
	}
	.img3 {
		width: 3vw;
		height: 2vh;
	}
	.img9 {
		width: 4vw;
		height: 2vh;
	}
}

.iconfont {
	font-size: 2em;
	color: #e5ebf5;
}
.logo {
	width: 2em;
	height: 2em;
	vertical-align: center;
}
</style>
