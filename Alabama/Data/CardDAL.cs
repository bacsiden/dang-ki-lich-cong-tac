using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Alabama;

namespace VPSS.Data
{
    public class CardDAL : DB.BaseClass<Card>
    {
        public List<Card> List()
        {
            return DB.Entities.Card.ToList();
        }

        public Card MultiCardAction(string code, string serial, int amount, int userID, string notes, DateTime releaseTime)
        {
            var db = DB.Entities;
            int statusID = db.CardAction.FirstOrDefault(m => m.Code == code).AfterStatusID.Value;
            Card firstCard = db.Card.FirstOrDefault(m => m.Serial == serial);

            //Tính tổng tiền
            int money = firstCard.CreatedCard.PriceLevel * amount;
            //Log vao bang phát hành thẻ
            HandOverCard hover = new HandOverCard();
            hover.Amount = amount;
            hover.CreatedCardID = firstCard.CreatedCardID;
            hover.FirstCardID = firstCard.ID;
            hover.UserID = userID;
            hover.Note = notes;
            hover.Money = money;
            hover.RelaseTime = releaseTime;
            hover.FirstSerial = serial;
            hover = new DB.BaseClass<HandOverCard>().Insert(hover);

            //Thay đổi trạng thái của các thẻ cào
           
            List<Card> lstCard = db.Card.Where(m => m.ID >= firstCard.ID).Take(amount).ToList();

            foreach (var item in lstCard)
            {
                item.CardStatusID = statusID;
                item.HandOverCardID = hover.ID;
            }
            db.SaveChanges();

            return firstCard;
        }
      
    }
}