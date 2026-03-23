// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deezer_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DeezerResponse<T> _$DeezerResponseFromJson<T>(
  Map<String, dynamic> json,
  T Function(Object? json) fromJsonT,
) => DeezerResponse<T>(
  data: (json['data'] as List<dynamic>).map(fromJsonT).toList(),
  total: (json['total'] as num?)?.toInt(),
  next: json['next'] as String?,
);

Map<String, dynamic> _$DeezerResponseToJson<T>(
  DeezerResponse<T> instance,
  Object? Function(T value) toJsonT,
) => <String, dynamic>{
  'data': instance.data.map(toJsonT).toList(),
  'total': instance.total,
  'next': instance.next,
};
