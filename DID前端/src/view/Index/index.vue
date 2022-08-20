<template>
	<div>
		<div class="container">
			<img class="img4" src="../../../public/img/4.png" alt="" />
			<img class="img5" src="../../../public/img/5.png" alt="" />
			<!-- <img class="img7" src="../../../public/img/7.png" alt="" /> -->
			<div class="renzheng" @click="ksrz">{{ $t('home.ksrz') }}</div>
			<div class="renzheng"><img class="img11" src="../../../public/img/11.png" alt="" /> {{ $t('home.sfyrz') }}</div>
			<div class="wenzi">{{ $t('home.xtjj') }}<img class="img8" src="../../../public/img/8.png" alt="" /></div>
			<div class="wenzi1">
				{{ $t('home.jjxq') }}
			</div>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			show: false,
			flag1: false //认证
		};
	},
	methods: {
		ksrz() {
			this.$router.push({ path: '/accountstate/erroridentity' });
		}
	}
};
</script>

<style lang="less" scoped>
.container {
	width: 100%;
	height: auto;
	background-color: #0e1a34;

	display: flex;
	align-items: center;
	justify-content: space-around;
	flex-direction: column;
	padding: 1.3rem 0 0 0;
	background-position: center 4rem;
	background-repeat: no-repeat;
	background-image: url('../../img/6.png');
	box-sizing: border-box;
	.img4 {
		margin-bottom: 1rem;
	}
	.img5 {
		margin-top: 1rem;
	}
	.img7 {
		width: 5.65rem;
		height: 0.9rem;
		margin-top: 5rem;
	}
	.wenzi {
		margin-top: 2rem;
		color: white;
		text-align: center;
		width: 2.5rem;
		height: auto;
		position: relative;
		font-size: 0.6rem;
		.img8 {
			width: 2.5rem;
			height: 0.3rem;
			height: auto;
			position: absolute;
			bottom: 0;
			left: 0;
		}
	}
	.wenzi1 {
		color: #91979e;
		padding: 2vh 8vw;
		box-sizing: border-box;
		line-height: 8vw;
		text-indent: 6.4vw;
		font-size: 0.5rem;
	}
}
.renzheng {
	width: 5.65rem;
	height: 0.9rem;
	border: 2px solid #2f7aec;
	border-radius: 0.2rem;
	line-height: 0.9rem;
	color: #f8fffe;
	display: block;
	text-align: center;
	font-size: 0.4rem;
	background-color: #122d56;
	img {
		width: 25px;
		height: 25px;
		vertical-align: middle;
	}
}
</style>
