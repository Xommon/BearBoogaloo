using UnityEngine;

public static class Language
{
    public static string[,] language = 
    {
        {"0", "0", "0", "٠"},
        {"1", "1", "1", "١"},
        {"2", "2", "2", "٢"},
        {"3", "3", "3", "٣"},
        {"4", "4", "4", "٤"},
        {"5", "5", "5", "٥"},
        {"6", "6", "6", "٦"},
        {"7", "7", "7", "٧"},
        {"8", "8", "8", "٨"},
        {"9", "9", "9", "٩"},
        {"Start Game", "Commencer", "Komencu Ludon", ""}, // 10
        {"Max Bet", "Mise Max", "Maks Veto", ""},
        {"Boards", "Plateaux", "Tabuloj", ""},
        {"Game Mode", "Mode de Jeu", "Luda Reĝimo", ""},
        {"Difficulty", "Difficulté", "Malfacilo", ""},
        {"?", "?", "?", "?"}, // 15
        {"Randomise Existing Values", "Randomiser les Valeurs Existantes", "Randomigi Ekzistantajn Valorojn", ""},
        {"Set Empty Board Values To ", "Définir les Valeurs du Plateau Vide à ", "Agordu Malplenajn Tabulajn Valorojn al ", ""},
        {"English", "Français", "Esperanto", "العربية (١٢٣)"},
        {"Name", "Nom", "Nomo", ""},
        {"Language", "Langue", "Lingvo", ""}, //20
        {"Sound", "Effets Sonores", "Sono", ""},
        {"Music", "Musique", "Muziko", ""},
        {"Bear 1", "Ours 1", "Urso 1", ""},
        {"Easy", "Facile", "Facila", ""},
        {"Normal", "Normal", "Normala", ""}, // 25
        {"Hard", "Difficile", "Malfacile", ""},
        {"Master", "Maître", "Majstro", ""},
        {"Level", "Niveau", "Nivelo", ""},
        {"Welcome to Bear Boogaloo! This is a guide on how to play.\nUse your arrow keys/WASD to navigate the slideshow left and right.", "Bienvenue à Bear Boogaloo ! Ceci est un guide sur la façon de jouer.\nUtilisez vos touches fléchées/WASD/ZQSD pour parcourir le diaporama vers la gauche et la droite.", "Bonvenon al Bear Boogaloo! Ĉi tio estas gvidilo pri kiel ludi.\nUzu viajn sagoklavojn/WASD por navigi la bildaron maldekstren kaj dekstren.", ""},
        {"On your turn, place a bet on any board that isn't locked\n or full (less than MAX BET). Each bet earns you 1 point.", 
         "À votre tour, placez un pari sur un plateau qui n'est ni verrouillé\n ni plein (moins que la MISE MAX). Chaque pari vous rapporte 1 point.", 
         "Dum via vico, faru veton sur ajna tabulo kiu ne estas ŝlosita\n aŭ plena (malpli ol MAX VETO). Ĉiu veto donas al vi 1 poenton.", 
         ""}, // 30

        {"Then, click on a card to play it. The board that matches the card will change its value to the value on the card.", 
         "Ensuite, cliquez sur une carte pour la jouer. Le plateau correspondant à la carte changera sa valeur en celle de la carte.", 
         "Tiam, alklaku karton por ludi ĝin. La tabulo kiu kongruas kun la karto ŝanĝos sian valoron al tiu de la karto.", 
         ""},

        {"Locked boards cannot have their values changed or have any bets placed on them. The lock is removed when the player who played the lock has their next turn.", 
         "Les plateaux verrouillés ne peuvent pas changer de valeur ni recevoir de paris. Le verrou est retiré lorsque le joueur qui l'a placé joue son prochain tour.", 
         "Ŝlositaj tabuloj ne povas havi siajn valorojn ŝanĝitaj aŭ ricevi vetojn. La ŝlosilo estas forigita kiam la ludanto kiu metis ĝin havas sian sekvan vicon.", 
         ""},

        {"When multiple boards have a value, the board with the current lowest score will turn red.", 
         "Lorsque plusieurs plateaux ont une valeur, celui avec le score le plus bas devient rouge.", 
         "Kiam pluraj tabuloj havas valoron, la tabulo kun la plej malalta poentaro fariĝos ruĝa.", 
         ""},

        {"If every board has a value but multiple bottom boards are tied, the round will continue until the tie is broken.", 
         "Si tous les plateaux ont une valeur mais que plusieurs d'entre eux ont le même score le plus bas, le tour continue jusqu'à ce que l'égalité soit brisée.", 
         "Se ĉiuj tabuloj havas valoron sed pluraj malsupraj tabuloj estas egalaj, la rondo daŭros ĝis la egalecon estos rompita.", 
         ""},

        {"Once every board has a value, the board with the lowest score is removed and all of the bets on it are deleted.", 
         "Une fois que chaque plateau a une valeur, celui avec le score le plus bas est retiré et tous les paris dessus sont supprimés.", 
         "Post kiam ĉiu tabulo havas valoron, la tabulo kun la plej malalta poentaro estas forigita kaj ĉiuj vetoj sur ĝi estas forigitaj.", 
         ""}, // 35

        {"There are three rounds in a game. The player with the most bets after three boards are removed wins!", 
         "Il y a trois manches dans une partie. Le joueur avec le plus de paris après l'élimination de trois plateaux gagne!", 
         "Estas tri rondoj en ludo. La ludanto kun la plej multaj vetoj post kiam tri tabuloj estas forigitaj venkas!", 
         ""},
        {"", "", "", ""},
        {"", "", "", ""},
        {"", "", "", ""},
        {"", "", "", ""}, // 40
    };
}
