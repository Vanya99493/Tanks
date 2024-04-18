using System.Text;
using TankModule;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Text messageText;

        public void ActivatePreRoundMessage(int roundNumber)
        {
            messageText.text = $"ROUND {roundNumber}";
        }

        public void ActivateEndRoundMessage(int winnerTankIndex, Tank[] tanks)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(
                $"<color=#{ColorUtility.ToHtmlStringRGB(tanks[winnerTankIndex].Color)}>Player {tanks[winnerTankIndex].Id}</color> " +
                $"WINS THE ROUND");
            strBuilder.Append("\n\n\n");
            strBuilder.Append(
                $"<color=#{ColorUtility.ToHtmlStringRGB(tanks[0].Color)}>Player {tanks[0].Id}: " +
                $"{tanks[0].Wins} WINS\n");
            strBuilder.Append(
                $"<color=#{ColorUtility.ToHtmlStringRGB(tanks[1].Color)}>Player {tanks[1].Id}: " +
                $"{tanks[1].Wins} WINS");
            
            messageText.text = strBuilder.ToString();
        }

        public void DeactivateMessage()
        {
            messageText.text = "";
        }
    }
}