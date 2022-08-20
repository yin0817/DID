<template>
	<div>
		<div class="top">
			<van-icon size="1rem" name="search" class="iconfont icon-zuo" class-prefix="icon" @click="goBack" /> <span class="wenzi">{{ this.title }}</span>
		</div>
	</div>
</template>

<script>
export default {
	props: ['title'],
	methods: {
		goBack() {
			this.$emit('goBack');
		}
	},
	created() {
		console.log(this.title);
	}
};
</script>

<style lang="less" scoped>
.top {
	width: 100%;
	height: auto;
	background-color: #1b2945;
	padding: 0.2rem 0.2rem 0 0.2rem;
	box-sizing: border-box;
	display: flex;
	.icon-zuo {
		color: white;
	}
	.wenzi {
		color: white;
		align-self: center;
		margin-left: 3rem;
		font-weight: bold;
		font-size: 0.4rem;
	}
}
</style>
