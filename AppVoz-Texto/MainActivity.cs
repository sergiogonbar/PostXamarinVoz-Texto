using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Speech;

namespace AppVoz_Texto
{
    [Activity(Label = "MiAplicacionVoz-Texto", MainLauncher = true, Icon = "@drawable/icon")]

    public class MainActivity : Activity
    {
        //variables 
        private bool isRecording;
        private readonly int VOICE = 10;
        private TextView Texto;
        private Button BotonGrabar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // inicializamos a falso para que no grabe
            isRecording = false;

            // establece el layout activity
            SetContentView(Resource.Layout.activity_main);

            // accedemos a los id del layout
            BotonGrabar = FindViewById<Button>(Resource.Id.btnGrabar);
            Texto = FindViewById<TextView>(Resource.Id.Texto);

            
            //Comprueba si podemos realmente grabar.Si podemos, asigne el evento al botón
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // si no hay microfono no graba y deshabilita el boton de grabar
                var alert = new Android.App.AlertDialog.Builder(BotonGrabar.Context);
                alert.SetTitle("No se detecta microfono para grabar");
                alert.SetPositiveButton("OK", (sender, e) =>
                {
                    Texto.Text = "No hay microfono en el dispositivo";
                    BotonGrabar.Enabled = false;
                    return;
                });

                alert.Show();
            }
            else
                BotonGrabar.Click += delegate
                {
                    // cmabiamos el texto del boton
                    BotonGrabar.Text = "Fin Grabacion";
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        // creamos un intent y lanzamos la actividad
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                        // put a message on the modal dialog
                        //voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.messageSpeakNow));

                         //Si hay más de 1.5s de silencio, considere el discurso sobre
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                        // Para poder reconocer otros idiomas por ejmeplo el Aleman:
                        // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, VOICE);
                    }
                };
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = Texto.Text + matches[0];

                        // Limite de caracteres permitidos
                        if (textInput.Length > 500)
                            textInput = textInput.Substring(0, 500);
                        Texto.Text = textInput;
                    }
                    else
                        Texto.Text = "No se reconoce ";
                   
                    BotonGrabar.Text = "Seguir grabando";
                }
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }
    }
}