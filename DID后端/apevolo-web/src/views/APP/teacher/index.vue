<template>
  <div class="app-container">
    <!--  工具  -->
    <el-button size="small" type="primary" @click="edit(null)">添加</el-button>
    <!--表格渲染-->
    <el-table
      ref="table"
      v-loading="table.loading"
      :data="table.data"
      row-key="noticeId"
      style="width: 100%"
    >
      <el-table-column prop="name" label="教师名称">
        <template slot-scope="scope">
          <el-avatar style="margin-right: 5px;" size="small" :src="transformSrc(scope.row.headImage)" />
          {{ scope.row.name }}
        </template>
      </el-table-column>
      <el-table-column prop="blurb" label="教师简介">
        <template slot-scope="scope">
          <span class="line1">{{ scope.row.blurb }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="createDate" label="创建时间">
        <template slot-scope="scope">
          {{ transformDate(scope.row.createDate) }}
        </template>
      </el-table-column>
      <!--   编辑与删除   -->
      <el-table-column
        label="操作"
        width="180px"
        align="center"
        fixed="right"
      >
        <template slot-scope="scope">
          <el-button
            size="small"
            type="warning"
            @click="edit(scope.row)"
          >
            修改
          </el-button>
          <el-button
            size="small"
            type="danger"
            :loading="scope.row.loading"
            :disabled="scope.row.loading"
            @click="remove(scope.row)"
          >
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <edit ref="edit" @refresh="getTable" />
  </div>
</template>

<script>
import Edit from './module/form'
import { list, del } from '@/api/APP/teacher'
import { transformSrc, transformDate } from '@/utils'

export default {
  name: 'Notice',
  components: {
    Edit
  },
  data() {
    return {
      table: {
        loading: false,
        query: {
          title: ''
        },
        data: []
      }
    }
  },
  created() {
    this.getTable()
  },
  methods: {
    transformSrc,
    transformDate,
    getTable() {
      this.table.loading = true
      list().then(res => {
        this.table.data = res.items.map(item => ({ ...item, loading: false }))
      }).finally(() => {
        this.table.loading = false
      })
    },
    edit(data) {
      let dom = this.$refs.edit
      dom.toggle(true, data)
      dom = null
    },
    remove(data) {
      this.$confirm('确定删除此记录').then(res => {
        data.loading = true
        del(data.teacherId).then(res => {
          this.$message.success('删除成功！')
          this.getTable()
        }).catch(() => {
          this.$message.error('删除失败！')
        })
      }).catch(() => {
        console.log(2)
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
::v-deep .el-input-number .el-input__inner {
  text-align: left;
}
.line1 {
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
</style>
