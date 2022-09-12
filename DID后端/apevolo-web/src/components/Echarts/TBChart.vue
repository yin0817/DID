<template>
  <div>
    <el-form
      ref="form"
      :inline="true"
      :model="form"
      :rules="rules"
      size="small"
      label-width="66px"
    >
      <el-form-item label="类型">
        <el-select v-model="value" placeholder="请选择" style="width:100px">
          <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
      </el-form-item>
    </el-form>
    <el-tabs v-model="activeName" type="card" @tab-click="handleClick">
      <el-tab-pane label="环比" name="hb">
        <BarChart />
      </el-tab-pane>
      <el-tab-pane label="同比" name="tb">
        <BarChart />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import BarChart from '@/components/Echarts/BarChart'
import { debounce } from '@/utils'

export default {
  components: {
    BarChart
  },
  data() {
    return {
      options: [{
        value: 'day',
        label: '日'
      }, {
        value: 'week',
        label: '周'
      }, {
        value: 'month',
        label: '月'
      }],
      activeName: 'hb',
      value: 'day'
    }
  },
  mounted() {
    this.__resizeHandler = debounce(() => {
      if (this.chart) {
        this.chart.resize()
      }
    }, 100)
    window.addEventListener('resize', this.__resizeHandler)
  }
}
</script>
