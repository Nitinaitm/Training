<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="BatchLifecycle.ascx.cs"
    Inherits="Training.BatchLifecycle" %>

<style>
    .lifecycle-section{margin-top:16px;padding:14px;border:1px solid #dee2e6;border-radius:10px;background:#fff}.lifecycle-section:first-child{margin-top:0}.lifecycle-title{font-size:17px;font-weight:700;margin-bottom:14px;text-align:center;color:#212529}.lifecycle-section .stage-line{display:flex;align-items:flex-start;min-width:900px}.lifecycle-section .bl-item{flex:1;position:relative;text-align:center}.lifecycle-section .bl-item:not(:last-child):after{content:"";position:absolute;top:25px;left:50%;width:100%;height:4px;background:#dc3545;z-index:0}.lifecycle-section .bl-item.bl-done:not(:last-child):after{background:#198754}.lifecycle-section .bl-bubble{position:relative;z-index:1}.lifecycle-section .bl-label{font-size:12px;font-weight:600;line-height:1.25;padding:0 4px}.lifecycle-section .bl-state{font-size:10px;margin-top:3px}.session-cycle{background:#fff}\n    .bl-wrap {
        margin: 20px 0 25px;
        padding: 20px;
        border: 1px solid #dee2e6;
        border-radius: 12px;
        background: #f8f9fa;
    }

    .bl-title {
        font-size: 20px;
        font-weight: 700;
        margin-bottom: 18px;
        text-align: center;
        color: #212529;
    }

    .bl-scroll {
        overflow-x: auto;
        overflow-y: visible;
        padding: 8px 0 48px;
    }

    .bl-line {
        display: flex;
        align-items: flex-start;
        min-width: 900px;
    }

    .bl-item {
        flex: 1;
        position: relative;
        text-align: center;
    }

    .bl-item:not(:last-child):after {
        content: "";
        position: absolute;
        top: 25px;
        left: 50%;
        width: 100%;
        height: 4px;
        background: #dc3545;
        z-index: 0;
    }

    .bl-item.bl-done:not(:last-child):after {
        background: #198754;
    }

    .bl-item.bl-skipped:not(:last-child):after,
    .bl-item.bl-na:not(:last-child):after {
        background: #dc3545;
    }

    .bl-bubble {
        position: relative;
        z-index: 1;
        width: 52px;
        height: 52px;
        line-height: 46px;
        border-radius: 50%;
        margin: 0 auto 8px;
        background: #dc3545;
        color: #fff;
        font-weight: 700;
        font-size: 15px;
        border: 3px solid #fff;
        box-shadow: 0 0 0 1px #dc3545;
        cursor: help;
    }

    .bl-item.bl-done .bl-bubble {
        background: #198754;
        box-shadow: 0 0 0 1px #198754;
    }

    .bl-item.bl-na .bl-bubble,
    .bl-item.bl-skipped .bl-bubble {
        background: #adb5bd;
        box-shadow: 0 0 0 1px #adb5bd;
    }

    .bl-item.bl-partial .bl-bubble {
        box-shadow: 0 0 0 1px #198754;
    }

    .bl-label {
        font-size: 12px;
        font-weight: 600;
        line-height: 1.25;
        padding: 0 4px;
        color: #212529;
    }

    .bl-state {
        font-size: 10px;
        margin-top: 3px;
        color: #dc3545;
    }

    .bl-item.bl-done .bl-state {
        color: #198754;
    }

    .bl-item.bl-na .bl-state,
    .bl-item.bl-skipped .bl-state {
        color: #6c757d;
    }

    .bl-bubble[data-tooltip] {
        position: relative;
    }

    .bl-bubble[data-tooltip]:hover:after {
        content: attr(data-tooltip);
        position: absolute;
        left: 50%;
        top: calc(100% + 8px);
        bottom: auto;
        transform: translateX(-50%);
        background: #212529;
        color: #fff;
        padding: 7px 10px;
        border-radius: 6px;
        font-size: 12px;
        font-weight: 600;
        line-height: 1.2;
        white-space: nowrap;
        z-index: 1000;
        box-shadow: 0 3px 10px rgba(0,0,0,.25);
    }

    .bl-bubble[data-tooltip]:hover:before {
        content: "";
        position: absolute;
        left: 50%;
        top: calc(100% + 2px);
        bottom: auto;
        transform: translateX(-50%);
        border: 6px solid transparent;
        border-top-color: transparent;
        border-bottom-color: #212529;
        z-index: 1001;
    }

    @media(max-width:768px) {
        .bl-wrap {
            padding: 15px;
        }

        .bl-scroll {
            margin-left: -5px;
            margin-right: -5px;
        }

        .bl-line {
            min-width: 760px;
        }

        .bl-bubble {
            width: 48px;
            height: 48px;
            line-height: 42px;
        }
    }
</style>

<div id="pnlLifecycle"
     runat="server"
     class="bl-wrap"
     visible="false">

    <div class="bl-title">
        Batch Life Cycle
    </div>

    <div class="bl-scroll">
        <asp:Literal ID="litLifecycle" runat="server" />
    </div>

</div>