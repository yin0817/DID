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
		<div class="yuyan">
			<span
				@click="
					show1 = !show1;
					show = false;
				"
				>语言切换</span
			>
		</div>
		<van-popup v-model="show" get-container="#nav" position="left" class="pop">
			<div class="content">
				<div class="item" v-for="(item, index) in list" :key="index" @click="skip(index)">
					<p>
						{{ item.title }}
					</p>
					<img src="../public/img/10.png" alt="" />
				</div>
			</div>
		</van-popup>
		<van-popup v-model="show1" get-container="#nav" position="right" class="pop1" :overlay="false">
			<div class="content">
				<div class="item" v-for="(item, index) in list1" :key="index" @click="listClick(index)">
					<p>
						{{ item.title }}
					</p>
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
			show1: false,
			flag2: true,
			list1: [{ title: this.$t('yuyan.Chinese') }, { title: this.$t('yuyan.English') }],
			list: [
				{ title: this.$t('list.title0'), event: 'https://eotc.im' },
				{ title: this.$t('list.title1'), event: 'https://fi.eotc.im/' },
				{ title: this.$t('list.title2') },
				{ title: this.$t('list.title3') },
				{ title: this.$t('list.title4') },
				{ title: this.$t('list.title5') },
				{ title: this.$t('list.title6') },
				{ title: this.$t('list.title7'), event: 'https://nft.eotc.im/' },
				{ title: this.$t('list.title8') },
				{ title: this.$t('list.title9') }
			]
		};
	},
	methods: {
		listClick(index) {
			switch (index) {
				case 0:
					this.$i18n.locale = 'zh';
					break;
				case 1:
					this.$i18n.locale = 'en';
					break;
				default:
					break;
			}
			this.show1 = false;
			this.show;
			var obj = this.$t('list');
			var arr = [];
			for (const key in obj) {
				arr.push(obj[key]);
			}
			arr.forEach((item, index) => {
				this.list[index].title = item;
			});
			var obj = this.$t('yuyan');
			var arr = [];
			for (const key in obj) {
				arr.push(obj[key]);
			}
			arr.forEach((item, index) => {
				this.list1[index].title = item;
			});
		},
		changeLang() {
			this.$i18n.locale == 'en' ? (this.$i18n.locale = 'zh') : (this.$i18n.locale = 'en');
			var obj = this.$t('list');
			var arr = [];
			for (const key in obj) {
				arr.push(obj[key]);
			}
			arr.forEach((item, index) => {
				this.list[index].title = item;
			});
		},
		gohome() {
			this.show = false;
			this.$router.push({ path: '/index' });
		},
		onClickLeft() {
			this.show = !this.show;
			this.show1 = false;
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
		this.$store.commit('setZH', 1);
	},
	watch: {
		list: {
			handle: function (val) {
				console.log(val);
			},
			deep: true
		}
	},
	computed: {
		xxx() {
			return this.$t('list');
		}
	}
};
</script>

<style lang="less" scoped>
.yuyan {
	width: auto;
	height: auto;
	position: fixed;
	top: 30px;
	left: 65%;
	color: white;
	font-size: 0.3rem;
	z-index: 999;
}
/deep/.van-nav-bar__title {
	display: flex !important;
	align-content: center;
}

/deep/.van-nav-bar {
	width: 100%;
	background-color: #1b2945;
	box-sizing: border-box;
	height: auto;
}
/deep/.van-overlay {
	z-index: 9 !important;
}
/deep/ .van-popup--left {
	top: calc(50% + 92px);
}
/deep/ .van-popup--right {
	top: calc(50% + 92px);
}

.pop {
	width: 80%;
	height: 100vh;
	z-index: 10 !important;
	background: #1b2946;

	.content {
		div {
			height: 100px;
			line-height: 100px;
			margin: 0 48px;
			border-bottom: 1px solid #2b374f;
			font-size: 28px;
			color: #b4b7c2;
			display: flex;
			justify-content: space-between;
			align-items: center;
			img {
				width: 32px;
				height: 32px;
			}
		}
	}
}
.pop1 {
	width: 50%;
	height: 100vh;
	z-index: 10 !important;
	background: #1b2946;

	.content {
		div {
			height: 100px;
			line-height: 100px;
			margin: 0 48px;
			border-bottom: 1px solid #2b374f;
			font-size: 28px;
			color: #b4b7c2;
			display: flex;
			justify-content: space-between;
			align-items: center;
			img {
				width: 32px;
				height: 32px;
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
#nav {
	height: 100vh;
	overflow: hidden;
	overflow-y: auto;
}
</style>
