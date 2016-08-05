namespace Dandaan

open System
open System.Collections.Generic
open System.Diagnostics
open System.Windows.Forms

type ProgramFS =
    static member Title = "مدیریت دندانپزشکی"
    static member DataDirectory = Application.StartupPath


module FS =
    let run (cmd : string) = Process.Start cmd
