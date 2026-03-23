// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_save_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumSaveResponse _$AlbumSaveResponseFromJson(Map<String, dynamic> json) =>
    AlbumSaveResponse(
      albumId: (json['albumId'] as num?)?.toInt(),
      albumCreated: json['albumCreated'] as bool,
      savedTracksCount: (json['savedTracksCount'] as num).toInt(),
      existingTracksCount: (json['existingTracksCount'] as num).toInt(),
    );

Map<String, dynamic> _$AlbumSaveResponseToJson(AlbumSaveResponse instance) =>
    <String, dynamic>{
      'albumId': instance.albumId,
      'albumCreated': instance.albumCreated,
      'savedTracksCount': instance.savedTracksCount,
      'existingTracksCount': instance.existingTracksCount,
    };
