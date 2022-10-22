<template>
  <el-dialog
    append-to-body
    :title="isEdit ? '编辑' : '添加'"
    :visible.sync='show'
    :close-on-click-modal="false"
    :before-close='reset'
    width="800px"
  >
    <el-form
      ref="form"
      :model="form.data"
      :rules="form.rules"
      size="small"
      label-width="80px"
    >
      <el-row :gutter='15'>
        <el-col :span='12'>
          <el-form-item
            label="课程名称"
            prop="name"
          >
            <el-input
              v-model="form.data.name"
              placeholder='请输入课程名称'
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :span='12'>
          <el-form-item
            label="课程价格"
            prop="price"
          >
            <el-input
              v-model="form.data.price"
              clearable
              placeholder='请输入课程价格'
            >
              <template slot="prepend">￥</template>
            </el-input>
          </el-form-item>
        </el-col>
        <el-col :span='12'>
          <el-form-item
            label="课程等级"
            prop="grade"
          >
            <el-select style='width: 100%;' v-model='form.data.grade' filterable clearables>
              <el-option label='初级' :value='0'>初级</el-option>
              <el-option label='中级' :value='1'>中级</el-option>
              <el-option label='高级' :value='2'>高级</el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span='12'>
          <el-form-item
            label="主讲讲师"
            prop="teacherId"
          >
            <el-select
              v-model='form.data.teacherId'
              filterable
              clearables
              style='width: 100%;'
            >
              <el-option
                v-for='item in teacher'
                :key='item.teacherId'
                :label='item.name'
                :value='item.teacherId'
              >
                {{item.name}}
              </el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span='12'>
          <el-form-item
            label="课程简介"
            prop="blurb"
          >
            <el-input
              v-model="form.data.blurb"
              clearable
              placeholder='请输入课程简介'
              type='textarea'
              resize='none'
              :rows='6'
            ></el-input>
          </el-form-item>
        </el-col>
        <el-col :span='12'>
          <el-form-item
            label="主讲内容"
            prop="content"
          >
            <el-input
              v-model="form.data.content"
              clearable
              placeholder='请输入主讲内容'
              type='textarea'
              resize='none'
              :rows='6'
            ></el-input>
          </el-form-item>
        </el-col>
        <el-col :span='24'>
          <el-form-item
            label="课程图片"
            prop="images"
          >
            <el-row>
              <el-col :span='6'>
                <el-upload
                  multiple
                  action=""
                  list-type="picture-card"
                  :show-file-list='false'
                  :file-list='form.fileList'
                  :before-upload='beforeUpload'
                  :http-request='onUpload'
                  :on-success='handleSuccess'
                  :on-error='onErrorUpload'
                >
                  <i class="el-icon-plus"></i>
                </el-upload>
              </el-col>
              <el-col :span='18'>
                <div class='img__list'>
                  <div class='item' v-for='item in form.data.images' :key='item'>
                    <img
                      class="img"
                      :src="transformSrc(item)"
                      alt=""
                    />
                    <div class="mask">
                      <span
                        class="icon delete"
                        @click="handleRemove(item)"
                      >
                        <i class="el-icon-delete"></i>
                      </span>
                    </div>
                  </div>
                </div>
              </el-col>
            </el-row>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <div
      slot="footer"
      class="dialog-footer"
    >
      <el-button
        type="text"
        @click="toggle(false)"
      >
        取消
      </el-button>
      <el-button
        :loading="form.loading"
        :disabled="form.loading"
        type="primary"
        @click="submit"
      >
        确认
      </el-button>
    </div>
  </el-dialog>
</template>

<script>
import { edit, upload } from '@/api/APP/course'
import { list } from '@/api/APP/teacher'
import { transformSrc } from '@/utils'

export default {
  data() {
    const validateUploadImage = (rule, val, cb) => {
      if (!val.length) {
        return cb(new Error('请上传课程相关图片！'))
      } else {
        return cb()
      }
    }
    return {
      show: false,
      isEdit: false,
      teacher: [],
      form: {
        loading: false,
        fileList: [],
        data: {
          name: '',
          grade: '',
          price: '',
          blurb: '',
          content: '',
          images: [],
          teacherId: ''
        },
        rules: {
          name: [{ required: true, message: '请输入公告标题', trigger: 'blur' }],
          price: [{ required: true, message: '请输入课程价格', trigger: 'blur' }],
          grade: [{ required: true, message: '请选择课程等级', trigger: 'blur' }],
          teacherId: [{ required: true, message: '请选择课程讲师', trigger: 'blur' }],
          blurb: [{ required: true, message: '请输入课程简介', trigger: 'blur' }],
          content: [{ required: true, message: '请输入主讲内容', trigger: 'blur' }],
          images: [{ required: true, validator: validateUploadImage, message: '请上传课程相关图片', trigger: 'blur' }]
        }
      }
    }
  },
  created() {
    this.getTeacher()
  },
  methods: {
    transformSrc,
    toggle(show, data) {
      !show && this.reset()
      this.isEdit = !!data
      this.show = show
      !!data && this.$nextTick().then(() => {
        Object.assign(this.form.data, { ...data, images: data.images.split(',') })
      })
    },
    // 重置
    reset(done) {
      this.$refs.form.resetFields()
      this.isEdit = false
      this.form.fileList = []
      done && done()
    },
    getTeacher() {
      list().then(res => {
        this.teacher = res.items
      })
    },
    beforeUpload(file) {
      const isIMG = file.type.includes('image/')
      const isLt2M = file.size / 1024 / 1024 < 4

      if (!isIMG) {
        this.$message.error('头像只能是图片!')
      }
      if (!isLt2M) {
        this.$message.error('上传头像图片大小不能超过 4MB!')
      }
      return isIMG && isLt2M
    },
    async onUpload({ file }) {
      const formData = new FormData()
      formData.append('file', file)
      return await upload(formData)
    },
    handleSuccess(res) {
      this.form.data.images.push(res.message)
    },
    onErrorUpload() {
      this.$message.error('图片上传失败，请重新上传！')
    },
    handleRemove(data) {
      this.form.data.images.splice(this.form.data.images.findIndex(item => item === data), 1)
    },
    handlePictureCardPreview(file) {},
    submit() {
      this.$refs.form.validate(valid => {
        if (valid) {
          this.form.loading = true
          edit({ ...this.form.data, images: this.form.data.images.join(',') }, this.isEdit).then(() => {
            this.$message.success(`${this.isEdit ? '编辑' : '添加'}成功！`)
            this.toggle(false)
            this.$emit('refresh')
          }).catch(() => {
            this.$message.error(`${this.isEdit ? '编辑' : '添加'}失败！`)
          }).finally(() => {
            this.form.loading = false
          })
        }
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
  .img__list {
    display: flex;
    align-items: center;
    min-width: 0;
    padding: 15px;
    border: 1px dashed #c0ccda;
    height: 148px;
    box-sizing: border-box;
    background-color: #fbfdff;
    border-radius: 6px;
    width: 100%;
    overflow-x: auto;
    .item {
      position: relative;
      flex: 0 0 148px;
      height: 100%;
      overflow: hidden;
      &:not(:last-of-type) {
        margin-right: 30px;
      }
      &:hover {
        .mask {
          display: flex;
        }
      }
      .img {
        display: block;
        width: 100%;
      }
      .mask {
        display: none;
        justify-content: center;
        align-items: center;
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, .5);
      }
      .icon {
        color: #CCC;
        cursor: pointer;
        &:hover {
          color: #FFF;
        }
      }
    }
  }
 ::v-deep {
   .el-input-number .el-input__inner {
     text-align: left;
   }
   .ql-container {
     height: 300px;
   }
 }
</style>
