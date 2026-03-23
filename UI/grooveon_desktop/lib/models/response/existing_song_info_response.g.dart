// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'existing_song_info_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ExistingSongInfoResponse _$ExistingSongInfoResponseFromJson(
  Map<String, dynamic> json,
) => ExistingSongInfoResponse(
  id: (json['id'] as num).toInt(),
  externalTrackId: json['externalTrackId'] as String?,
  title: json['title'] as String,
  artistName: json['artistName'] as String,
  albumTitle: json['albumTitle'] as String?,
  coverUrl: json['coverUrl'] as String?,
);

Map<String, dynamic> _$ExistingSongInfoResponseToJson(
  ExistingSongInfoResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'externalTrackId': instance.externalTrackId,
  'title': instance.title,
  'artistName': instance.artistName,
  'albumTitle': instance.albumTitle,
  'coverUrl': instance.coverUrl,
};
