using System.Collections.Generic;

namespace SteamReviews.Services
{
	public static class SteamMessages
	{
		public static readonly Dictionary<string, string> Texts = new Dictionary<string, string>
		{
			{ "italian", "Ciao!\nStiamo facendo un gioco simile e potrebbe piacerti!\nPer favore, dai un'occhiata alla pagina del nostro gioco.\nSe ti piace, aggiungi il gioco alla tua lista dei desideri, ciò ci aiuterà nella promozione!\nScusaci per il disturbo e grazie mille!\n\nGioco:" },
			{ "polish", "https://us06web.zoom.us/rec/share/Xx0gYVr_LWD-lo6oUaFj6HlJrGqAinxd4ljQ37kbkXFZ57JKSVY4FDq7mDLCeFok.IwrNADG6X1p2KSr_?startTime=1702649447000!\nRobimy podobną grę, która może ci się spodobać!\nProszę, sprawdź stronę naszej gry.\nJeśli ci się podoba, dodaj ją do listy życzeń, pomoże nam to w promocji!\nPrzepraszamy za kłopot i dziękujemy!\n\nGra:" },
			{ "schinese", "你好！\n我们正在制作一款类似的游戏，也许你会喜欢！\n请查看我们游戏的页面。\n如果喜欢，请将游戏添加到愿望列表，这将有助于推广！\n为打扰你感到抱歉，非常感谢！\n\n游戏：" },
			{ "tchinese", "你好！\n我們正在製作一款類似的遊戲，也許你會喜歡！\n請查看我們遊戲的頁面。\n如果喜歡，請將遊戲添加到願望列表，這將有助於推廣！\n為打擾你感到抱歉，非常感謝！\n\n遊戲：" },
			{ "japanese", "こんにちは！\n似たようなゲームを作っていて、もしかしたら気に入っていただけるかもしれません！\n当社のゲームのページをご覧ください。\n気に入った場合は、ゲームをウィッシュリストに追加していただけると、プロモーションに役立ちます！\nご迷惑をおかけして申し訳ありません、どうもありがとうございます！\n\nゲーム：" },
			{ "koreana", "안녕하세요!\n우리는 비슷한 게임을 만들고 있어, 아마도 마음에 드실 거에요!\n저희 게임 페이지를 확인해보세요.\n마음에 들면 게임을 위시리스트에 추가해주세요, 이는 홍보에 도움이 될 거예요!\n불편을 끼쳐드려 죄송하고 감사합니다!\n\n게임:" },
			{ "thai", "สวัสดีครับ/ค่ะ!\nเรากำลังทำเกมที่คล้ายกันและอาจจะถูกใจคุณ!\nโปรดดูหน้าเกมของเรา\nหากคุณชอบ กรุณาเพิ่มเกมลงในรายการที่ต้องการ เพื่อช่วยสนับสนุนการโปรโมท!\nขออภัยในความไม่สะดวกและขอบคุณมากครับ/ค่ะ!\n\nเกม:" },
			{ "bulgarian", "Здравейте!\nНие правим подобна игра и вероятно ще ви хареса!\nМоля, разгледайте страницата на нашата игра.\nАко ви хареса, добавете я в списъка с желания, това ще помогне за промоцията!\nИзвиняваме се за притеснението и ви благодарим много!\n\nИгра:" },
			{ "czech", "Ahoj!\nDěláme podobnou hru a možná se vám bude líbit!\nProsím, podívejte se na stránku naší hry.\nPokud se vám líbí, přidejte hru do seznamu přání, pomůže nám to s propagací!\nOmlouváme se za obtěžování a děkujeme vám moc!\n\nHra:" },
			{ "danish", "Hej!\nVi laver et lignende spil, og måske vil du også kunne lide det!\nTag et kig på vores spils side, hvis du vil.\nHvis du kan lide det, så tilføj spillet til din ønskeliste; det vil hjælpe med at fremme det!\nUndskyld for besværet og tak skal du have!\n\nSpil:" },
			{ "german", "Hallo!\nWir machen ein ähnliches Spiel und vielleicht gefällt es dir auch!\nBitte schau dir die Seite unseres Spiels an.\nWenn es dir gefällt, füge das Spiel bitte deiner Wunschliste hinzu, das hilft bei der Promotion!\nEntschuldige die Störung und vielen Dank!\n\nSpiel:" },
			{ "english", "Hello!\nWe are making a similar game, and perhaps you will like it too!\nPlease check out our game's page.\nIf you like it, please add the game to your wishlist; it will help in promotion!\nSorry for the inconvenience, and thank you so much!\n\nGame:" },
			{ "spanish", "¡Hola!\nEstamos haciendo un juego similar y tal vez te guste también.\nPor favor, visita la página de nuestro juego.\nSi te gusta, añade el juego a tu lista de deseos, ¡ayudará en la promoción!\nDisculpa las molestias y ¡muchas gracias!\n\nJuego:" },
			{ "latam", "¡Hola!\nEstamos haciendo un juego similar y quizás te guste también.\nPor favor, visita la página de nuestro juego.\nSi te gusta, añade el juego a tu lista de deseos, ¡ayudará en la promoción!\nDisculpa las molestias y ¡muchas gracias!\n\nJuego:" },
			{ "greek", "Γεια σας!\nΦτιάχνουμε ένα παρόμοιο παιχνίδι και ίσως σας αρέσει επίσης!\nΠαρακαλώ, ελέγξτε τη σελίδα του παιχνιδιού μας.\nΑν σας αρέσει, παρακαλούμε προσθέστε το παιχνίδι στη λίστα επιθυμιών σας, θα βοηθήσει στην προώθηση!\nΣυγγνώμη για την ταλαιπωρία και ευχαριστώ πολύ!\n\nΠαιχνίδι:" },
			{ "french", "Salut !\nNous faisons un jeu similaire et peut-être qu'il te plaira aussi !\nMerci de consulter la page de notre jeu.\nSi tu aimes, ajoute le jeu à ta liste de souhaits, cela aidera à la promotion !\nDésolé pour le dérangement et merci beaucoup !\n\nJeu :" },
			{ "hungarian", "Helló!\nHasonló játékot készítünk, és talán neked is tetszeni fog!\nKérlek, nézd meg a játékunk oldalát.\nHa tetszik, add hozzá a játékot a kívánságlistádhoz, ez segít a promócióban!\nElnézést a kellemetlenségért és köszönjük szépen!\n\nJáték:" },
			{ "dutch", "Hallo!\nWe maken een vergelijkbaar spel en misschien vind je het ook leuk!\nBekijk alsjeblieft de pagina van ons spel.\nAls het je bevalt, voeg het spel dan toe aan je verlanglijstje; het zal helpen bij de promotie!\nSorry voor het ongemak en hartelijk dank!\n\nSpel:" },
			{ "norwegian", "Hei!\nVi lager et lignende spill, og kanskje vil du også like det!\nVennligst sjekk ut spill-siden vår.\nHvis du liker det, vennligst legg til spillet på ønskelisten din; det vil hjelpe med promoteringen!\nUnnskyld for bryderiet og tusen takk!\n\nSpill:" },
			{ "portuguese", "Oi!\nEstamos fazendo um jogo semelhante e talvez você também goste!\nPor favor, confira a página do nosso jogo.\nSe gostar, adicione o jogo à sua lista de desejos; isso ajudará na promoção!\nDesculpe pelo incômodo e muito obrigado!\n\nJogo:" },
			{ "brazilian", "Oi!\nEstamos fazendo um jogo semelhante e talvez você também goste!\nPor favor, confira a página do nosso jogo.\nSe gostar, adicione o jogo à sua lista de desejos; isso ajudará na promoção!\nDesculpe pelo incômodo e muito obrigado!\n\nJogo:" },
			{ "romanian", "Salut!\nFacem un joc similar și poate îți va plăcea și ție!\nTe rog să verifici pagina jocului nostru.\nDacă îți place, te rog adaugă jocul în lista ta de dorințe, asta va ajuta la promovare!\nNe pare rău pentru inconveniențe și îți mulțumim mult!\n\nJoc:" },
			{ "russian", "Привет!\nМы делаем похожую игру и, возможно, она тебе тоже понравится!\nПожалуйста, посмотри страницу нашей игры.\nЕсли понравится – пожалуйста, добавь игру в список желаемого, это поможет в продвижении!\nИзвини за беспокойство и огромное тебе спасибо!\n\nИгра:" },
			{ "finnish", "Hei!\nTeemme samankaltaista peliä, ja ehkä pidät siitä myös!\nKatsohan pelimme sivu.\nJos se miellyttää, lisää peli toivelistallesi, se auttaa promootiossa!\nAnteeksi häiriöstä ja kiitos paljon!\n\nPeli:" },
			{ "swedish", "Hej!\nVi gör ett liknande spel, och kanske gillar du det också!\nVänligen kolla in vår spelesida.\nOm du gillar det, lägg till spelet på din önskelista; det hjälper till med marknadsföringen!\nFörlåt för besväret och tack så mycket!\n\nSpel:" },
			{ "turkish", "Merhaba!\nBenzer bir oyun yapıyoruz ve belki de sana da hoş gelebilir!\nLütfen oyunumuzun sayfasını kontrol et.\nEğer beğenirsen, lütfen oyunu dilek listene ekle; bu tanıtıma yardımcı olur!\nRahatsızlık için özür dileriz ve çok teşekkür ederiz!\n\nOyun:" },
			{ "vietnamese", "Chào bạn!\nChúng tôi đang làm một trò chơi tương tự và có thể bạn cũng sẽ thích nó!\nHãy xem trang trò chơi của chúng tôi.\nNếu bạn thích – vui lòng thêm trò chơi vào danh sách mong muốn, điều này sẽ giúp quảng bá!\nXin lỗi vì sự phiền phức và cảm ơn bạn rất nhiều!\n\nTrò chơi:" },
			{ "ukrainian", "Привіт!\nМи робимо схожу гру і, можливо, вона тобі також сподобається!\nБудь ласка, переглянь сторінку нашої гри.\nЯкщо сподобається – будь ласка, додай гру до списку бажаного, це допоможе у продвиженні!\nВибач за непокій і велике тобі спасибі!\n\nГра:" }
		};
	}
}
