// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'music_search_item_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MusicSearchItemResponse _$MusicSearchItemResponseFromJson(
  Map<String, dynamic> json,
) => MusicSearchItemResponse(
  type: json['type'] as String,
  id: (json['id'] as num).toInt(),
  title: json['title'] as String,
  subtitle: json['subtitle'] as String?,
  imageUrl: json['imageUrl'] as String?,
  previewUrl: json['previewUrl'] as String?,
  artistId: (json['artistId'] as num?)?.toInt(),
  albumId: (json['albumId'] as num?)?.toInt(),
  externalTrackId: json['externalTrackId'] as String?,
);

Map<String, dynamic> _$MusicSearchItemResponseToJson(
  MusicSearchItemResponse instance,
) => <String, dynamic>{
  'type': instance.type,
  'id': instance.id,
  'title': instance.title,
  'subtitle': instance.subtitle,
  'imageUrl': instance.imageUrl,
  'previewUrl': instance.previewUrl,
  'artistId': instance.artistId,
  'albumId': instance.albumId,
  'externalTrackId': instance.externalTrackId,
};
