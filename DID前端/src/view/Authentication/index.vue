<template>
	<div id="container">
		<!-- 认证信息 -->
		<div class="box">
			<div class="top">
				<div class="left">
					<img src="../../../public/img/2.png" alt="" />
				</div>
				<div class="right">
					<span class="rz">{{ $t('authentication.wrz') }}</span
					><span>UID123456</span>
				</div>
			</div>
			<div class="bottom">
				<div class="left">
					<img src="../../../public/img/11.png" alt="" />
					<span>{{ $t('authentication.sfwrz') }}</span>
				</div>
				<div class="right" @click="xyfClick">信用分 <span class="wenzi">0</span></div>
			</div>
		</div>
		<!-- cell 列表 -->
		<div class="list">
			<van-cell-group :border="false">
				<van-cell class="cell" :border="false">
					<template #title>
						<van-icon name="search" class="iconfont icon-shenhe" class-prefix="icon" />
						<span class="custom-title">认证审核</span>
						<span class="dian"></span>
					</template>
					<template #right-icon>
						<van-icon name="search" class="iconfont icon-shang" class-prefix="icon" v-if="flag" @click="flag = !flag" />
						<van-icon name="search" class="iconfont icon-xia" class-prefix="icon" v-if="!flag" @click="flag = !flag" />
					</template>
				</van-cell>
				<div v-show="!flag">
					<van-cell class="cell" :border="false">
						<template #title>
							<van-icon name="search" class="iconfont icon-shenhe yincang" class-prefix="icon" />
							<span class="custom-title">推荐人审核</span>
						</template>
						<template #right-icon>
							<van-icon name="search" class="iconfont icon-you" class-prefix="icon" />
						</template>
					</van-cell>
					<van-cell class="cell">
						<template #title>
							<van-icon name="search" class="iconfont icon-shenhe yincang" class-prefix="icon" />
							<span class="custom-title">上级节点审核</span>
						</template>
						<template #right-icon>
							<van-icon name="search" class="iconfont icon-you" class-prefix="icon" />
						</template>
					</van-cell>
					<van-cell class="cell">
						<template #title>
							<van-icon name="search" class="iconfont icon-shenhe yincang" class-prefix="icon" />
							<span class="custom-title">中高级节点审核</span>
						</template>
						<template #right-icon>
							<van-icon name="search" class="iconfont icon-you" class-prefix="icon" />
						</template>
					</van-cell>
				</div>

				<van-cell class="cell" v-for="(item, index) in list" :key="index" :border="false">
					<template #title>
						<van-icon name="search" :class="'iconfont ' + item.icon" class-prefix="icon" />
						<span class="custom-title">{{ item.name }}</span>
					</template>
					<template #right-icon>
						<div v-if="item.name == '身份信息'" class="sfxx"><span style="color: #848c90">未认证</span><span style="color: #aa5342">审核失败</span><span style="color: #2a8bff">审核中</span></div>
						<router-link :to="'/' + item.url"> <van-icon name="search" class="iconfont icon-you" class-prefix="icon" /></router-link>
					</template>
				</van-cell>
			</van-cell-group>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			list: [
				{ icon: 'icon-xinyongfen', name: '信用分明细' },
				{ icon: 'icon-lianxifangshi', name: '身份信息' },
				{ icon: 'icon-fukuanxinxi', name: '收付款信息', url: 'receiving' },
				{ icon: 'icon-bangdinggongliandizhi', name: '各公链绑定地址', url: 'chain' },
				{ icon: 'icon-bangdingxiangmu', name: '绑定各项目' },
				{ icon: 'icon-lianxifangshi', name: '各社区联系方式' }
			],
			flag: true,
			height: null
		};
	},
	methods: {
		jiantou() {
			this.flag = !this.flag;
		},
		xyfClick() {
			this.$router.push({ path: '/credit' });
		}
	},
	created() {
		this.height = document.documentElement.clientHeight;
	}
};
</script>

<style lang="less" scoped>
.yincang {
	visibility: hidden;
}
/deep/ .cell {
	width: 90vw;
	display: flex;
	justify-content: space-between;
	height: 10vw;
	align-items: center;
	img {
		width: 5vw;
		height: 5vw;
		margin-right: 5vw;
	}
}
.box {
	width: 90%;
	height: 5rem;
	background: linear-gradient(to right, #2a8bff, #51d3fa);
	border-radius: 0.2rem;
	padding: 0.3rem 0.2rem;
	box-sizing: border-box;
	margin: 5%;

	.top {
		display: flex;
		.left {
			width: 2rem;
			height: 2rem;
			border-radius: 50%;
			background-color: white;
			img {
				width: 1.2rem;
				height: 1.2rem;
			}
			display: flex;
			justify-content: center;
			align-items: center;
		}
		.right {
			display: flex;
			flex-direction: column;
			height: 1.2rem;
			justify-content: space-around;
			margin-left: 0.25rem;
			color: white;
			margin-top: 0.3rem;
			font-size: 0.5rem;
			.rz {
				font-weight: 700;
			}
		}
	}
	.bottom {
		display: flex;
		margin-top: 0.5rem;
		justify-content: space-between;
		.left {
			display: flex;
			justify-content: center;
			height: 1rem;
			align-items: center;
			img {
				width: 0.6rem;
				height: 0.6rem;
				margin-right: 0.2rem;
			}
			font-size: 0.6rem;
			opacity: 0.5;
			color: white;
		}
		.right {
			width: 4rem;
			height: 1rem;
			border-radius: 1rem;
			border: 0.015rem solid white;
			background-color: white;
			display: flex;
			justify-content: center;
			align-items: center;
			color: #2483f7;
			font-size: 0.5rem;
			.wenzi {
				font-weight: 700;
				margin-left: 0.2rem;
			}
		}
	}
}
.list {
	margin-top: 1rem;
	/deep/.van-cell-group {
		//	border: none !important;
	}
	/deep/.cell {
		position: relative;
		//	border: none !important;
	}
	/deep/.van-cell::after {
		border: none !important;
	}
	/deep/.van-cell--borderless {
		border: none !important;
	}
	/deep/.van-cell {
		width: 100%;
		background-color: #111a39 !important;
		padding: 0.1rem 0.4rem !important;
		//		border: none !important;
	}
	.iconfont {
		color: white !important;
		//	background-color: red !important;
	}
	.dian {
		display: block;
		width: 2vw;
		height: 2vw;
		background-color: red;
		border-radius: 50%;
		position: absolute;
		top: 2vw;
		left: 24vw;
	}
	.custom-title {
		color: white;
		margin-left: 3vw;
	}
}
#container {
	width: 100%;
	height: calc(100vh - 92px) !important;

	background-color: #111a39;
	overflow: hidden;
	overflow-y: auto;
}
</style>
