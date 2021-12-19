# WebServer
Nodejs uygulamasıdır.
Web uygulamasının backend tarafıdır.
Web arayüzünde ihtiyaç duyulan bilgilerin repo'dan istendiği ve kullanıcı değişimlerini de oraya kayıt eden köprü uygulamadır.
API fonksiyonları içermektedir. Web arayüzünün ihtiyaçlarına bağlı olarak değişmektedir.
routes/webapi.js dosyası altında tüm fonksiyonlar yönetilir.
routes/index.js ile build edilen react app yayınlanır. Geri kalan tüm aktiviteler api üstünden gerçekleştirilir.
Arayüz ihtiyacına bağlı olarak api fonksiyonları değişmektedir.
Her fonksiyon için genel kalıp;
	- isteği anla - repo isteği oluştur - viewModel oluştur - arayüze json olarak gönder

