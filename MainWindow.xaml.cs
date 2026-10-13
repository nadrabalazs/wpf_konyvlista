using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Konyvlista;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        FrissitDarabszam();
    }

    // Kész segítség: a listaelemek számát írja ki.
    private void FrissitDarabszam()
    {
        txtAllapot.Text = $"Könyvek száma: {lstKonyvek.Items.Count}";
    }

    private void Felvetel_Click(object sender, RoutedEventArgs e)
    {
        // C1: adatellenőrzés, ismétlődés kizárása, felvétel, alaphelyzet.
        string cim = txtCim.Text.Trim();
        string szerzo = txtSzerzo.Text.Trim();

        if (cim == "" || szerzo == "" || cim.Contains('|') || szerzo.Contains('|'))
        {
            MessageBox.Show("Adja meg a címet és a szerzőt!\nA | jel nem használható.", "Hibás adat", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        foreach (var item in lstKonyvek.Items)
        {
            string[] reszek = item.ToString().Split(" | ");

            if (reszek[0] == cim && reszek[1] == szerzo)
            {
                MessageBox.Show("Ez a könyv már szerepel a listában!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        string allapot = chkElolvasva.IsChecked == true ? "elolvasva" : "olvasásra vár";
        lstKonyvek.Items.Add($"{cim} | {szerzo} | {allapot}");
        txtCim.Clear();
        txtSzerzo.Clear();
        chkElolvasva.IsChecked = false;

        FrissitDarabszam();
    }

    private void Torles_Click(object sender, RoutedEventArgs e)
    {
        // C2: kijelölés ellenőrzése, megerősítés, törlés, darabszám.
        if (lstKonyvek.SelectedIndex == -1)
        {
            MessageBox.Show("Válasszon ki egy elemet a törléshez!", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        MessageBoxResult valasz = MessageBox.Show("Biztosan törölni szeretné a kijelölt könyvet?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (valasz == MessageBoxResult.Yes)
        {
            lstKonyvek.Items.RemoveAt(lstKonyvek.SelectedIndex);
            FrissitDarabszam();
        }
    }

    private void Mentes_Click(object sender, RoutedEventArgs e)
    {
        // C3: fájlválasztás, UTF-8 mentés, kivételkezelés.
        SaveFileDialog sfd = new SaveFileDialog
        {
            Filter = "Szövegfájl (*.txt)|*.txt",
            FileName = "konyvek.txt"
        };
        if (sfd.ShowDialog() == false)
            return;
        try
        {
            StreamWriter sw = new StreamWriter(sfd.FileName, append: false, Encoding.UTF8);
            for (int i = 0; i < lstKonyvek.Items.Count; i++)
            {
                sw.WriteLine(lstKonyvek.Items[i]);
            }
            sw.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    private void Betoltes_Click(object sender, RoutedEventArgs e)
    {
        // C4: fájlválasztás, UTF-8 beolvasás, listacsere, kivételkezelés.
        // Az adattárolás módját szabadon választhatja meg.
        OpenFileDialog ofd = new OpenFileDialog
        {
            Filter = "Szövegfájl (*.txt)|*.txt"
        };
        if (ofd.ShowDialog() == false)
            return;
        string[] sorok;
        try
        {
            sorok = File.ReadAllLines(ofd.FileName, Encoding.UTF8);
            
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
        lstKonyvek.Items.Clear();
        foreach (string sor in sorok)
        {
            if (sor != "")
            {
                lstKonyvek.Items.Add(sor);
            }
        }
        FrissitDarabszam();
    }

}
