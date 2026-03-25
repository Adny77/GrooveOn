// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'music_overview_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MusicOverviewRequest _$MusicOverviewRequestFromJson(
  Map<String, dynamic> json,
) => MusicOverviewRequest(
  mode: json['mode'] as String,
  userId: (json['userId'] as num).toInt(),
  year: (json['year'] as num).toInt(),
  month: (json['month'] as num?)?.toInt(),
  take: (json['take'] as num?)?.toInt() ?? 4,
);

Map<String, dynamic> _$MusicOverviewRequestToJson(
  MusicOverviewRequest instance,
) => <String, dynamic>{
  'mode': instance.mode,
  'userId': instance.userId,
  'year': instance.year,
  'month': instance.month,
  'take': instance.take,
};
