using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.LegacyInputHelpers;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text PPoints;
    public Text DPoints;
    public Text TextProb3;
    public Text TextProb4;
    public Text TextProb5;

    public int[] values = new int[52];
    int cardIndex = 0;
    private int sum = 0;
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        List<int> values2 = new List<int>();;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                values2.Add(j + 1);
            }

            for (int k = 0; k < 4; k++)
            {
                values2.Add(10);
            }
        }

        values = values2.ToArray();
        
        for (int a = 0; a < faces.Length; a++)
        {
            if ((a == 0) || (a % 13 == 0))
            {
                values[a] = 11;
            }

        }
        /*
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        
    }

    private void ShuffleCards()
    {
        Sprite tempf;
        int tempval;
        int randomIndex;
        for (int i = 0; i < faces.Length; i++)
        {
            randomIndex = Random.Range(0, faces.Length);
            tempf = faces[i];
            tempval = values[i];
            faces[i] = faces[randomIndex];
            faces[randomIndex] = tempf;
            values[i] = values[randomIndex];
            values[randomIndex] = tempval;
           
        }
        /*
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */       
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            
        }
        if (dealer.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "JUGADOR PIERDE";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "JUGADOR GANA";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        int cartastotal = 0;
        float pplayer = player.GetComponent<CardHand>().points;
        float mas21 = 0;
        float entre17y21 = 0;
        float dealermayorplayer = 0;

        for (int i = cardIndex + 1; i <faces.Length; i++)
        {
            // Probabilidad de que el dealer tenga más puntuación que el jugador
            cartastotal++;
            if (values[3] + values[i] > pplayer)
            {
                dealermayorplayer++;
            }
            // Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
            if (values[i] + pplayer >= 17 && values[i] + pplayer <= 21)
            {
                entre17y21++;
            }
            // Probabilidad de que el jugador obtenga más de 21 si pide una carta
            if (values[i] + pplayer > 21)
            {
                mas21++;
            }
        }
        TextProb3.text = (dealermayorplayer/cartastotal).ToString();
        TextProb4.text = (entre17y21/cartastotal).ToString();
        TextProb5.text = (mas21/cartastotal).ToString();
    }


    void PushDealer()
    {
        /*
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
        //TODO: quitar Log
        PPoints.text = "Puntos jugador: "+player.GetComponent<CardHand>().points.ToString();


    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        //Repartimos carta al jugador
        PushPlayer();
        CalculateProbabilities();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "JUGADOR PIERDE";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "JUGADOR GANA";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);  
        hitButton.interactable = false;
        stickButton.interactable = false;
        
        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }
        
        if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "JUGADOR GANA";
            Debug.Log(1);
        }
        else
        {
            if (player.GetComponent<CardHand>().points == dealer.GetComponent<CardHand>().points)
            {
                finalMessage.text = "EMPATE";
                Debug.Log(2);
            }
            else
            {

                if (player.GetComponent<CardHand>().points > dealer.GetComponent<CardHand>().points)
                {
                    finalMessage.text = "JUGADOR GANA";
                    Debug.Log(3);
                }
                else
                {
                    finalMessage.text = "JUGADOR PIERDE";
                    Debug.Log(4);
                }
            }
        }

        
        
        
        DPoints.text = "Puntos dealer: "+dealer.GetComponent<CardHand>().points.ToString();

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        PPoints.text = "";
        DPoints.text = "";
        TextProb3.text = "";
        TextProb4.text = "";
        TextProb5.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}
