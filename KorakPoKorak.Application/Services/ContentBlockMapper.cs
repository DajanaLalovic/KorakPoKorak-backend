using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    internal static class ContentBlockMapper
    {
        public static ContentBlockDto ToDto(ContentBlock block) => new()
        {
            Id = block.Id,
            Type = block.MediaAsset.Type,
            Url = block.MediaAsset.Url,
            Content = block.MediaAsset.Content,
            MimeType = block.MediaAsset.MimeType,
            FileName = block.MediaAsset.FileName,
            OrderIndex = block.OrderIndex,
            CreatedAt = block.MediaAsset.CreatedAt
        };

        public static List<ContentBlockDto> ToDtoList(IEnumerable<ContentBlock>? blocks)
        {
            if (blocks == null)
                return new List<ContentBlockDto>();

            return blocks
                .OrderBy(b => b.OrderIndex)
                .ThenBy(b => b.Id)
                .Select(ToDto)
                .ToList();
        }

        public static void ValidateInputs(IEnumerable<ContentBlockInputDto> blocks)
        {
            foreach (var block in blocks)
            {
                if (!Enum.IsDefined(typeof(MediaType), block.Type))
                    throw new ArgumentException("Content block type is invalid.");

                if (!string.IsNullOrWhiteSpace(block.Url) &&
                    block.Url.Trim().StartsWith("blob:", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        "Browser blob: URLs cannot be stored. Upload the file first and use the returned server URL.");
                }

                switch (block.Type)
                {
                    case MediaType.TEXT:
                        if (string.IsNullOrWhiteSpace(block.Content))
                            throw new ArgumentException("TEXT content blocks require content text.");
                        break;
                    case MediaType.VIDEO:
                    case MediaType.IMAGE:
                    case MediaType.AUDIO:
                    case MediaType.FILE:
                        if (string.IsNullOrWhiteSpace(block.Url))
                            throw new ArgumentException($"{block.Type} content blocks require a url.");
                        break;
                }
            }
        }

        public static List<(MediaAsset Asset, int OrderIndex)> BuildAssets(
            IEnumerable<ContentBlockInputDto> blocks,
            int createdById)
        {
            ValidateInputs(blocks);

            var now = DateTime.UtcNow;
            return blocks
                .OrderBy(b => b.OrderIndex)
                .Select(b => (
                    new MediaAsset
                    {
                        Type = b.Type,
                        Url = string.IsNullOrWhiteSpace(b.Url) ? null : b.Url.Trim(),
                        Content = string.IsNullOrWhiteSpace(b.Content) ? null : b.Content,
                        MimeType = string.IsNullOrWhiteSpace(b.MimeType) ? null : b.MimeType.Trim(),
                        FileName = string.IsNullOrWhiteSpace(b.FileName) ? null : b.FileName.Trim(),
                        CreatedAt = now,
                        CreatedById = createdById
                    },
                    b.OrderIndex
                ))
                .ToList();
        }
    }
}
