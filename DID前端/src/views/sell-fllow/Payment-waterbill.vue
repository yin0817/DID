<template>
  <div class="water-bill">
    <!-- 流水审查页面 -->
    <header class="header" @click="$router.back(-1)">
      <van-icon name="arrow-left" />
      <span class="hd-txt">
        <!-- 商家姓名 -->
        {{ MerchanInfo.sname }}
      </span>
    </header>
    <section class="water-bill-content">
      <div class="top-content">
        待收款&nbsp;
        <van-tag
          type="warning"
          size="large"
          v-if="!isReminders && +item.dsx === 1"
          @click="urge_payMoney"
          >催付款</van-tag
        >
        <van-tag type="primary" v-if="isReminders" size="large"
          >已催付款</van-tag
        >
      </div>
      <div class="top-mid-content">
        <span class="price">
          <!-- 总金额 -->
          ￥{{ MerchanInfo.amount1 }}
        </span>
        <span class="mid-r-content">
          <!-- <van-button size="mini" >确认收款</van-button> -->
        </span>
      </div>
    </section>
    <section class="water-bill-tips">
      <span><van-icon name="warn-o" /></span>
      <span :style="{ marginLeft: '15px' }"
        >收款请认准订单详情中展示的买家收款账号，或买家分享
        的付款账号卡片。其余买家私聊发送的账号信息一律不要接受。
      </span>
    </section>

    <article class="dialogue" ref="dialogue-content">
      <van-pull-refresh
        v-model="isLoading_before"
        @refresh="get_chatting_records"
      >
        <!-- 卖家信息展示 -->
        <template v-for="(message, index) in megList">
          <!-- 商家信息 -->
          <div
            ref="dialogue-content-child"
            class="seller-sends-info mesInfo"
            v-if="message.role === 'seller' && message.message"
            :key="index"
          >
            <span class="author-img">{{ item.sname.slice(0, 1) }}</span>
            <span class="author-info">
              <p class="payAuthor-name">{{ item.sname }}</p>
              <span
                class="payAuthor-content breakword"
                v-content:[message]
              ></span>
            </span>
          </div>

          <!-- 用户信息 -->
          <div
            ref="dialogue-content-child"
            class="buyer-sends-info mesInfo"
            :key="index"
            v-else-if="message.role === 'buyer' && message.message"
          >
            <span class="author-img">
              <!-- 用户头像 -->
              {{ myname.slice(0, 1) }}
            </span>
            <span class="author-info breakword">
              <span
                class="payAuthor-content breakword"
                v-content:[message]
              >
              </span>
            </span>
          </div>
        </template>
      </van-pull-refresh>
    </article>
    <van-form
      ref="footer"
      class="footer"
      @submit="onSend"
      :show-error="false"
      validate-trigger="onSubmit"
    >
      <van-field
        v-model="message"
        @focus="ipt_focus"
        @blur="ipt_blur"
        name="message"
        :rules="rules"
        autocomplete="off"
        rows="1"
        autofocus
        autosize
        clearable
        type="textarea"
        maxlength="80"
      >
      </van-field>
      <van-button
        :disabled="isSend"
        size="small"
        native-type="submit"
        type="primary"
      >
        {{ isSend ? "连线中" : "发送" }}
      </van-button>
    </van-form>
  </div>
</template>

<script>
import {  getItem } from "@/utils/storage";
import { GetOidMsg } from "@/api/payverification";

import { Reminders } from "@/api/trxRequest";
import { domain } from "@/utils/request";
import waterBillSeller from "@/mixins/water-bill-seller";

export default {
  name: "Payment-waterbill",
  props: ["item", "MerchanInfo"],
  mixins: [waterBillSeller],
  data() {
    return {
      isSend: true,
      page: 1,
      myname: localStorage.getItem("uname"),
      isLoading_before: false,
      time: 20 * 60 * 1000,
      message: "",
      rules: [
        {
          required: true,
          validator: this.validator,
          message: "发送内容不能为空",
        },
      ],
      megList: [],
      curRole: "",
      token: "",
      iscreate_DefaultMs: true,
      isReminders: false,
    };
  },
  directives: {
    content: {
      bind(el, binding) {
        if (binding.arg.html) {
          el.innerHTML = binding.arg.message;
        } else {
          el.innerText = binding.arg.message;
        }
      },
    },
  },
  created() {
    const tokenObj = getItem(`cs-${this.MerchanInfo.odid}`);
    this.curRole = this.$route.query.role;
    this.get_token(tokenObj, 0);
  },
  methods: {
    ipt_focus() {
      // this.$refs["footer"].$el.style.marginBottom = "0px";
    },
    ipt_blur() {
      // this.$refs["footer"].$el.style.marginBottom = "0";
    },
    async urge_payMoney() {
      try {
        await Reminders({
          did: this.MerchanInfo.odid,
          mail: this.item.mail,
          type: 2, // 用户催商家付款
        });
        this.$toast.success(
          <div>
            <p style="font-size:13px;margin:5px">已发送邮件至对方邮箱！</p>
          </div>
        );
        this.isReminders = true;
        // <p style="font-size:15px;margin:5px">买家正在快马加鞭的给您进行汇款!</p>
        this.megList.push(
          this.cinit_mes("buyer", "我转币至合约，请尽快完成订单支付！", true)
        );
        this.soket.send("我转币至合约，请尽快完成订单支付！");
        this.messageScrollIntoView();
      } catch (err) {
        this.isReminders = true;
        this.$toast.error("请不要频繁催促对方！！！");
      }
    },
    messageScrollIntoView() {
      this.$nextTick(() => {
        if (this.$refs["dialogue-content-child"] instanceof Array) {
          this.$refs["dialogue-content-child"]?.slice(-1)[0]?.scrollIntoView();
        }
      });
    },
    init_pagewater_bill() {
      const role = this.$route.query.role;
      if (role === "buyer") {
        const url = `wss://${domain}/api/wss/Connect?token=${this.token}`;
        //console.log(url);
        this.soket = new WebSocket(url);
        this.soket.onopen = () => {
          this.$toast.clear();
          //连接成功 发送默认消息 只有首次会创建并发送
          if (this.iscreate_DefaultMs) {
            this.init_mesgList();
          }
          this.iscreate_DefaultMs = false;
          window.addEventListener("hashchange", () => {
            this.soket.close();
            console.warn("您已断开连接");
          });
          //console.log("连接成功 Connected!");
        };
        // 监听离线消息
        this.soket.onmessage = (result) => {
          //console.log("result", result);
          //console.log(result.data);
          this.megList.push(this.cinit_mes("seller", result.data, true));
        };
        this.soket.onerror = function (error) {
          console.warn("连接错误 error");
          //console.log(error.data);
        };
        this.soket.onclose = () => {
          console.warn("连接错误，断开连接。。");
        };
      }
    },
    onSend() {
      if (this.soket.readyState == WebSocket.OPEN) {
        this.megList.push(this.cinit_mes("buyer", this.message, false));
        this.soket.send(this.message);
      }
      this.messageScrollIntoView();
      this.message = "";
    },
    cinit_mes(role, message, htmlflg, date, serial) {
      return {
        role,
        message,
        html: htmlflg,
        date: date ?? this.transformTime_Zh(Date.now()),
        pid: serial ?? 0,
      };
    },
    init_mesgList() {
      this.megList.push(
        this.cinit_mes("seller", this.item.smes, false),
        this.cinit_mes(
          "seller",
          `联系方式
             <a href="tel:${this.MerchanInfo.wechat}">${this.MerchanInfo.wechat}</a>\n
              <a href="mailto:${this.item.mail}">${this.item.mail}</a>`,
          true
        )
      );
      try {
        this.messageScrollIntoView();
        this.isSend = false;
      } catch {
        this.isSend = true;
      }
    },
    async get_chatting_records() {
      let page = this.page;
      try {
        const { data } = await GetOidMsg({
          oid: this.MerchanInfo.odid,
          page: this.page,
        });
        if (data.length > 0) {
          const newDataList = [];
          while (data.length > 0) {
            const firstMsg = data.shift();
            const role = firstMsg.user === "0" ? "buyer" : "seller";
            newDataList.push(
              this.cinit_mes(
                role,
                firstMsg.msg,
                false,
                firstMsg.date,
                firstMsg.uid
              )
            );
          }
          Promise.resolve()
            .then(() => {
              //console.log("newDataList", newDataList);
              if (page === 1) this.megList = [];
              return "首次刷新数据";
            })
            .then(() => {
              this.megList.unshift(...newDataList.reverse());
            })
            .finally(() => {
              if (page === 1) this.init_mesgList();
              this.isLoading_before = false;
              this.page++;
            });
        } else {
          this.isLoading_before = false;
        }
      } catch (err) {
        this.isLoading_before = false;
        console.log(err);
      }
    },
    validator(value) {
      if ((value ?? "") === "") {
        //console.log(value);
        return false;
      }
      return true;
    },
  },
};
</script>

<style lang="less" scoped>
.water-bill {
  position: fixed;
  z-index: 99;
  background-color: #fff;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  /deep/ .van-pull-refresh {
    height: 100%;
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow-y: auto;
  }
  .header {
    padding: 25px 25px 10px;
    font-size: 0.45rem;
    margin-bottom: 15px;
    .hd-txt {
      font-weight: 700;
      margin-left: 25px;
    }
  }
  .water-bill-content {
    padding: 25px;
    background-color: #eee;
    font-size: 30px;

    .price {
      color: var(--main--color);
      font-weight: 700;
    }
    .top-mid-content {
      display: flex;
      padding: 15px 15px 0 0;
      align-items: center;
      span {
        flex: 1;
      }
      .mid-r-content {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        button {
          margin: 0 15px;
          background-color: rgb(196, 195, 195);
          padding: 25px;
          color: #000;
        }
        .pay-time {
          color: rgb(252, 184, 66);
        }
      }
    }
  }
  .water-bill-tips {
    padding: 25px;
    display: flex;
    background-color: rgb(255, 241, 230);
    color: rgb(252, 184, 66);
    font-size: 24px;
    span:first-child {
      width: 36px;
      font-size: 36px;
    }
  }

  .dialogue {
    padding: 25px;
    flex: 1;
    overflow-y: auto;
    // background-color: red;
    // 卖家信息展示
    .seller-sends-info {
      display: flex;
      font-size: 0.5rem;
      .author-img {
        background-color: rgb(111, 179, 250);
        color: #fff;
        line-height: 40px;
        font-size: 30px;
        text-align: center;
        width: 40px;
        height: 40px;
        padding: 15px;
        border-radius: 50%;
      }
      .author-info {
        flex: 1;
        max-width: 70%;
        display: flex;
        padding: 10px 15px;
        flex-direction: column;
        .payAuthor-name {
          font-size: 24px;
        }
        .payAuthor-content {
          font-size: 0.4rem;
          width: fit-content;
          background-color: #eee;
          margin: 15px 0;
          padding: 15px;
          line-height: 50px;
          border-radius: 15px;
          --webkit--word-wrap: break-word;
          max-width: 100%;
        }
      }
    }

    // 买家信息展示
    .buyer-sends-info {
      display: flex;
      font-size: 0.5rem;
      flex-direction: row-reverse;
      .author-img {
        background-color: rgb(38, 207, 225);
        color: #fff;
        line-height: 40px;
        font-size: 30px;
        text-align: center;
        width: 40px;
        height: 40px;
        padding: 15px;
        border-radius: 50%;
      }

      .author-info {
        max-width: 70%;
        display: flex;
        padding: 0 15px;
        .payAuthor-content {
          font-size: 0.4rem;
          flex: auto;
          width: fit-content;
          background-color: var(--main--color);
          color: #fff;
          margin: 15px 0;
          overflow: hidden;
          padding: 15px;
          --webkit--word-wrap: break-word;
          border-radius: 15px;
        }
      }
    }
  }
  /* 强制不换行 */
  .nowrap {
    white-space: nowrap;
  }
  /* 允许单词内断句，首先会尝试挪到下一行，看看下一行的宽度够不够，
不够的话就进行单词内的断句 */
  .breakword {
    word-wrap: break-word;
  }
  /* 断句时，不会把长单词挪到下一行，而是直接进行单词内的断句 */
  .breakAll {
    word-break: break-all;
  }
  /* 超出部分显示省略号 */
  .ellipsis {
    text-overflow: ellipsis;
    overflow: hidden;
  }

  .footer {
    background-color: rgb(247, 247, 247);
    padding: 15px 25px 45px 25px;
    position: relative;
    margin-bottom: 0;
    .van-cell {
      padding: 10px;
      width: auto;
      margin-right: 1.5rem;
    }
    button {
      position: absolute;
      right: 25px;
      top: 37%;
      transform: translateY(-50%);
    }

    /deep/ .van-field__control {
      padding-left: 25px;
    }
  }
}
</style>
