// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_duplicate_check_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongDuplicateCheckRequest _$SongDuplicateCheckRequestFromJson(
  Map<String, dynamic> json,
) => SongDuplicateCheckRequest(
  externalTrackIds: (json['externalTrackIds'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
);

Map<String, dynamic> _$SongDuplicateCheckRequestToJson(
  SongDuplicateCheckRequest instance,
) => <String, dynamic>{'externalTrackIds': instance.externalTrackIds};
