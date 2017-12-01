namespace OriginalDataForwarding.Modules.VariableClass
{
    /// <summary>
    /// 發送統計
    /// </summary>
    public class SendStatistics
    {
        /// <summary>
        /// 總發送數
        /// </summary>
        public int TotalSendCount { set; get; }

        /// <summary>
        /// 發送成功數
        /// </summary>
        public int SuccessSendCount { set; get; }

        /// <summary>
        /// 例外發生數
        /// </summary>
        public int ExceptionSendCount { set; get; }
    }
}
