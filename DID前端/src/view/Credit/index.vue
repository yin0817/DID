<template>
	<div class="content1">
		<!-- 信用图表 -->
		<titles @goBack="goBack" :title="title"></titles>
		<div class="grade">
			<div class="text">
				<p>
					<countTo :startVal="startVal" :endVal="endVal" :duration="duration"></countTo>
				</p>
				<p>信用评分低</p>
			</div>
			<div class="svgbox">
				<svg class="svg" viewBox="0 0 300 300">
					<circle cx="150" cy="90" r="75" stroke="#323E57" stroke-width="10" fill="none" stroke-dasharray="236" stroke-dashoffset="-236" stroke-linecap="round" />
					<circle class="square" cx="150" cy="90" r="75" stroke="#2E85F8" stroke-width="10" fill="none" stroke-dasharray="236 236" stroke-dashoffset="-236" stroke-linecap="round" />
				</svg>
			</div>

			<img src="../../../public/img/bg-footer.png" alt="" />
		</div>
		<!-- 信用分流水 -->
		<van-tabs v-model="active" background="#2E85F8" title-inactive-color="#8DC3F8" color="#ffffff" title-active-color="#FFFDFF">
			<van-tab title-class="'title'" title="加分记录">
				<div class="center">
					<div class="box" v-for="(item, index) in 10" :key="index">
						<div class="box-left">
							<p>完成强关系链身份认证</p>
							<p>20220.05.21 02:45</p>
						</div>
						<p class="box-right color-green">+8</p>
					</div>
				</div>
			</van-tab>
			<van-tab title="扣分记录">
				<div class="center">
					<div class="box" v-for="(item, index) in 10" :key="index">
						<div class="box-left">
							<p>被推荐人陈粒非恶意交易失败</p>
							<p>20220.05.21 02:45</p>
						</div>
						<p class="box-right color-orange">-1</p>
					</div>
				</div>
			</van-tab>
		</van-tabs>
	</div>
</template>

<script>
import countTo from 'vue-count-to';
import titles from '../../components/titlie.vue';
export default {
	name: 'credit',
	components: {
		countTo,
		titles
	},
	data() {
		return {
			title: '信用评分',
			// 需要滚动的时间
			duration: 3000,
			// 初始值
			startVal: 0,
			// 最终值
			endVal: 8,
			active: 1
		};
	},
	computed: {},
	mounted() {
		this.endVal = localStorage.getItem('myjifen');
	},
	methods: {
		goBack() {
			this.$router.back();
		}
	}
};
</script>

<style lang="less" scoped>
.title {
	font-weight: bold;
}
.content1 {
	width: 100%;
	height: 100vh;
	overflow: hidden;
	overflow-y: auto;
	/deep/ .van-nav-bar .van-icon {
		color: #fff;
	}
	/deep/ .van-nav-bar__title {
		color: #fff;
	}
	/deep/ [class*='van-hairline']::after {
		border: none;
	}
	.grade {
		display: flex;
		justify-content: center;
		background-color: #1b2945;
		position: relative;
		img {
			position: absolute;
			bottom: 0;
			width: 100%;
			height: 25vw;
		}
		.text {
			margin: 100px 0;
			text-align: center;
			color: #fff;
			p:first-child {
				font-size: 100px;
				// font-weight: bold;
			}
			p:last-child {
				margin-top: 30px;
				font-size: 28px;
			}
		}
		.svgbox {
			position: absolute;

			width: 540px;
			height: 540px;
			.svg {
				left: 25%;
				width: 100%;
				height: 100%;
				.square {
					animation: anim 3s linear 1;
				}

				@keyframes anim {
					0% {
						stroke-dasharray: 5 480;
					}
					100% {
						stroke-dasharray: 471;
					}
				}
			}
		}
	}
	.center {
		padding: 24px;
		height: auto;
		.box {
			background: #fff;
			padding: 22px 24px 22px 32px;
			margin-bottom: 20px;
			border-radius: 16px;
			display: flex;
			justify-content: space-between;
			.box-left {
				p:first-child {
					font-size: 36px;
					margin-bottom: 10px;
					font-weight: bold;
				}
				p:last-child {
					font-size: 28px;
					color: #999;
				}
			}
			.box-right {
				display: flex;
				align-items: center;
			}
			.color-green {
				color: #1d9c3f;
			}
			.color-orange {
				color: #fc7542;
			}
		}
	}
}
</style>
