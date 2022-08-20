<template>
  <div class="app">
    <Nav></Nav>
    <div class="contatiner">
      <div class="top">
        <transition-page>
            <router-view></router-view>
        </transition-page>
      </div>
    </div>
  </div>
</template>

<script>
import Nav from "@/components/Nav";

import transitionPage from "@/components/transitionPage";


export default {
  name: "home-index",
  components: {
    Nav,
    transitionPage,
  },
 
  data() {
    return {
    };
  },
  methods: {

  },
};
</script>
<style lang="less" scoped>
.app {
  width: 100vw;
  height: 100vh;
}
</style>
