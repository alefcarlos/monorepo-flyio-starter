using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flyio.Demo.AppHost;

public sealed class TerraformResource : Resource
{
    public TerraformResource(
        string name,
        string workingDirectory)
        : base(name)
    {
        WorkingDirectory = workingDirectory;
    }

    public string WorkingDirectory { get; }

    public async Task<ExecuteCommandResult> ExecuteAsync(
        ExecuteCommandContext context,
        params string[] arguments)
    {
        var notification = context.Services.GetRequiredService<ResourceNotificationService>();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "terraform",
                WorkingDirectory = WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        foreach (var argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        await notification.PublishUpdateAsync(
            this,
            state => state with
            {
                State = KnownResourceStates.Running,
                StartTimeStamp = DateTime.UtcNow
            });

        try
        {
            process.Start();

            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(context.CancellationToken);

            var output = await stdout;
            var error = await stderr;

            if (!string.IsNullOrWhiteSpace(output))
            {
                context.Logger.LogInformation("{Output}", output);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                context.Logger.LogError("{Error}", error);
            }

            return process.ExitCode == 0
                ? CommandResults.Success()
                : CommandResults.Failure(
                    $"terraform {string.Join(' ', arguments)} failed with exit code {process.ExitCode}");
        }
        finally
        {
            await notification.PublishUpdateAsync(
                this,
                state => state with
                {
                    State = KnownResourceStates.Finished,
                    StopTimeStamp = DateTime.UtcNow
                });
        }
    }
}

public static class TerraformExtensions
{
    public static IResourceBuilder<TerraformResource> AddTerraform(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        string workingDirectory)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(workingDirectory);

        var absoluteWorkingDirectory = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, workingDirectory));

        var resource = new TerraformResource(
            name,
            absoluteWorkingDirectory);

        var resourceBuilder = builder.AddResource(resource);

        resourceBuilder.WithInitialState(new CustomResourceSnapshot
        {
            ResourceType = "TerraformCommander",
            CreationTimeStamp = DateTime.UtcNow,
            State = KnownResourceStates.NotStarted,
            Properties = [
                new(CustomResourceKnownProperties.Source, "Terraform Commander")
            ]
        });

        resourceBuilder
            .WithCommand(
                "init",
                "Terraform Init",
                context => resource.ExecuteAsync(context, "init"),
                new CommandOptions
                {
                    IconName = "ArrowSync",
                    IconVariant = IconVariant.Filled
                })
            .WithCommand(
                "plan",
                "Terraform Plan",
                context => resource.ExecuteAsync(context, "plan"),
                new CommandOptions
                {
                    IconName = "DocumentSearch",
                    IconVariant = IconVariant.Filled
                })
            .WithCommand(
                "apply",
                "Terraform Apply",
                context => resource.ExecuteAsync(context, "apply", "-auto-approve"),
                new CommandOptions
                {
                    IconName = "Play",
                    IconVariant = IconVariant.Filled,
                    ConfirmationMessage =
                        "Are you sure you want to apply the Terraform infrastructure?"
                })
            .WithCommand(
                "destroy",
                "Terraform Destroy",
                context => resource.ExecuteAsync(context, "destroy", "-auto-approve"),
                new CommandOptions
                {
                    IconName = "Delete",
                    IconVariant = IconVariant.Filled,
                    ConfirmationMessage =
                        "Are you sure you want to destroy the Terraform infrastructure?"
                });

        return resourceBuilder;
    }
}